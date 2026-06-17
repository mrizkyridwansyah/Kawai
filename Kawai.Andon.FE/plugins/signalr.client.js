// plugins/signalr.client.js
import * as signalR from '@microsoft/signalr'

class UnlimitedRetryPolicy {
  nextRetryDelayInMilliseconds(retryContext) {
    const attempt = retryContext.previousRetryCount;

    if (attempt === 0) return 0; // Retry 1: langsung
    if (attempt === 1) return 5000; // Retry 2: 5 detik
    if (attempt === 2) return 10000; // Retry 3: 10 detik

    return 30000; // Retry 4 ke atas: 30 detik selamanya
  }
}

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig();
  const baseUrl = config.public.apiBase;

  /**
   * Buat koneksi SignalR ke path tertentu
   * @param {string} hubPath path hub, contoh: "/notifapprovalhub"
   * @returns {HubConnection}
   */
  const createSignalRConnection = async (hubPath) => {
    const fullUrl = `${baseUrl}${hubPath.startsWith('/') ? hubPath : '/' + hubPath}`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(fullUrl)
      .withAutomaticReconnect(new UnlimitedRetryPolicy())
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.onreconnecting((error) => {
      console.warn(`[SignalR] Reconnecting to ${hubPath}...`, error);
    });

    connection.onreconnected((connectionId) => {
      console.log(`[SignalR] Reconnected to ${hubPath}. Connection ID: ${connectionId}`);
    });

    connection.onclose((error) => {
      console.error(`[SignalR] Connection permanently closed from ${hubPath}:`, error);
      // Optional: bisa trigger notifikasi ke user atau auto-reconnect manual
    });

    try {
      await connection.start();
      console.log(`[SignalR] Connected to ${hubPath}`);
    } catch (err) {
      console.error(`[SignalR] Failed to connect to ${hubPath}:`, err);
    }

    return connection;
  };

  // Inject sebagai $createSignalR
  nuxtApp.provide('createSignalR', createSignalRConnection);
});
