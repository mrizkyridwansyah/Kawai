import http from 'k6/http';
import { sleep, check } from 'k6';
import { Trend, Counter } from 'k6/metrics';

// =========================================================
// METRICS (dikumpulkan otomatis oleh k6)
// =========================================================
export let requestTime = new Trend('request_duration');
export let status200 = new Counter('status_200');
export let status429 = new Counter('status_429');
export let status401 = new Counter('status_401');

// =========================================================
// USER LOGIN DATA
// =========================================================
const users = [
    { username: 'k61', password: '123' },
    { username: 'k62', password: '123' },
    { username: 'k63', password: '123' },
    { username: 'k64', password: '123' },
    { username: 'k65', password: '123' },
    { username: 'k66', password: '123' },
    { username: 'k67', password: '123' },
    { username: 'k68', password: '123' },
    { username: 'k69', password: '123' },
    { username: 'k610', password: '123' },
];

// =========================================================
// LOAD TEST STAGES
// =========================================================
export const options = {
    stages: [
        { duration: '10s', target: 10 }, // ramp-up ke 10 user
        { duration: '40s', target: 50 }, // naik ke 50 user
        { duration: '10s', target: 0 },  // turun ke 0
    ],
    thresholds: {
        http_req_duration: [
            'p(50)<200',
            'p(90)<400',
            'p(95)<600',
            'p(99)<1000'
        ],
        http_req_failed: ['rate<0.05'],
    },
};

// =========================================================
// SETUP – LOGIN SEMUA USER, SIMPAN TOKEN
// =========================================================
export function setup() {
    const tokens = [];

    for (let user of users) {

        const loginRes = http.post(
            'http://192.168.0.11:9100/api/auth/sign-in',
            JSON.stringify({
                username: user.username,
                password: user.password
            }),
            { headers: { 'Content-Type': 'application/json' } }
        );

        check(loginRes, {
            'login successful': (r) =>
                r.status === 200 &&
                r.json('Data.AccessToken') !== undefined
        });

        const token = loginRes.json('Data.AccessToken');

        if (token) tokens.push(token);
    }

    return { tokens };
}

// =========================================================
// DEFAULT FUNCTION – REQUEST UTAMA
// =========================================================
export default function (data) {
    const body = JSON.stringify({
        Page: 1,
        Length: 10,
        Sorts: {
            WarehouseName: "asc",
        },
        Filters: [
            {
                Keyword: "",
                FactoryCode: "0000",
            },
        ]
    });
    // Pilih token acak
    const token = data.tokens[Math.floor(Math.random() * data.tokens.length)];

    const headers = {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
    };

    const res = http.post(
        'http://192.168.0.11:9100/api/warehouse/list',
        body,
        { headers }
    );

    // Metrics
    requestTime.add(res.timings.duration);

    if (res.status === 200) status200.add(1);
    if (res.status === 429) status429.add(1);
    if (res.status === 401) status401.add(1);

    sleep(Math.random() + 0.5);
}

// =========================================================
// CUSTOM SUMMARY (100% bekerja)
// =========================================================
export function handleSummary(data) {
    const m = data.metrics;

    function safe(metric, field, decimals = 2) {
        if (!metric || !metric.values || metric.values[field] === undefined) {
            return "N/A";
        }
        return Number(metric.values[field]).toFixed(decimals);
    }

    const now = new Date();
    const timestamp =
        now.getFullYear() + "-" +
        String(now.getMonth() + 1).padStart(2, '0') + "-" +
        String(now.getDate()).padStart(2, '0') + "_" +
        String(now.getHours()).padStart(2, '0') + "-" +
        String(now.getMinutes()).padStart(2, '0') + "-" +
        String(now.getSeconds()).padStart(2, '0');

    // filename final
    const filename = `../../results/warehouse-list_${timestamp}.json`;


    return {
        stdout: `
------------------------------------------------------
                 CUSTOM SUMMARY K6
------------------------------------------------------
Total Requests     : ${safe(m.http_reqs, "count", 0)}
2xx Success        : ${safe(m.status_200, "count", 0)}
429 Rate Limited   : ${safe(m.status_429, "count", 0)}
401 Unauthorized   : ${safe(m.status_401, "count", 0)}

Avg Duration       : ${safe(m.request_duration, "avg")}
Max Duration       : ${safe(m.request_duration, "max")}

p(50)              : ${safe(m.http_req_duration, "p(50)")}
p(90)              : ${safe(m.http_req_duration, "p(90)")}
p(95)              : ${safe(m.http_req_duration, "p(95)")}
p(99)              : ${safe(m.http_req_duration, "p(99)")}

Success Rate       : ${m.status_200?.values && m.http_reqs?.values
                ? ((m.status_200.values.count / m.http_reqs.values.count) * 100).toFixed(2) + " %"
                : "N/A"
            }
------------------------------------------------------
`,
        [filename]: JSON.stringify(data, null, 2)
    };
}
