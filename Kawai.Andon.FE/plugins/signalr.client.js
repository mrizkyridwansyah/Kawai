// plugins/signalr.client.js
import * as signalR from '@microsoft/signalr'
import configs from '@/app.config.json'

export default defineNuxtPlugin((nuxtApp) => {
  const baseUrl = process.env.NODE_ENV === 'production'
    ? configs.baseUrl
    : configs.baseUrlDev; // fallback jika Dev kosong

  /**
   * Buat koneksi SignalR ke path tertentu
   * @param {string} hubPath path hub, contoh: "/notifapprovalhub"
   * @returns {HubConnection}
   */
  const createSignalRConnection = async (hubPath) => {
    const fullUrl = `${baseUrl}${hubPath.startsWith('/') ? hubPath : '/' + hubPath}`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(fullUrl)
      .configureLogging(signalR.LogLevel.Information)
      .build();

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
