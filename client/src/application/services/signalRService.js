// src/application/services/signalRService.js
import * as signalR from '@microsoft/signalr';

class SignalRService {
  constructor() {
    this.connection = null;
  }

  async startConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5124/pixelHub', {
        withCredentials: true,
      })
      .configureLogging(signalR.LogLevel.Trace)
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(() => {
      console.log('Reconnected to SignalR hub.');
    });

   

    try {
      await this.connection.start();
      if(this.connection){
          this.connection.invoke("OnConnectedAsync", "nbr");
      }
      console.log('SignalR Connected.');
    } catch (err) {
      console.error('Error establishing SignalR connection:', err);
    }
  }

  on(eventName, callback) {
    if (this.connection) {
      this.connection.on(eventName, callback);
    }
  }

  off(eventName, callback) {
    if (this.connection) {
      this.connection.off(eventName, callback);
    }
  }

  invoke(eventName, data) {
    if (this.connection) {
      this.connection.invoke(eventName, data).catch((err) => {
        console.error(`Error invoking '${eventName}':`, err);
      });
    }
  }
}

const signalRService = new SignalRService();
export default signalRService;
