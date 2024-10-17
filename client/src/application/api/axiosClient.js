// src/api/axiosClient.js
import axios from 'axios';

const axiosClient = axios.create({
  baseURL: 'https://jsonplaceholder.typicode.com/', // Example API
  headers: {
    'Content-Type': 'application/json',
  },
});

export default axiosClient;
