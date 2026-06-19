const fs = require('fs');
const env = process.argv[2] || 'DEFAULT';
const envFile = process.argv[3] || '.env';

let api = '-';
try {
  const content = fs.readFileSync(envFile, 'utf8');
  const match = content.match(/NUXT_PUBLIC_API_BASE="?([^"\n]+)/);
  if (match) api = match[1];
} catch (e) {}

const now = new Date().toLocaleString('id-ID', { timeZone: 'Asia/Jakarta' });

fs.writeFileSync('.output/public/BUILD_INFO.txt',
`Environment: ${env}
Built At: ${now}
API Base: ${api}
`);

console.log(`BUILD_INFO.txt created [${env}]`);
