import axios from "axios";
const baseConfig = {
  baseURL: "http://localhost:9000",
};

export default {
  service: (useAuth) => {
    const instance = axios.create(baseConfig);
    instance.defaults.headers.common["Content-Type"] = "application/json";
    if (useAuth) {
      instance.interceptors.request.use(
        async (config) => {
          const token = localStorage.getItem("jwt");
          if (token) {
            config.headers = {
              ...config.headers,
              Authorization: `Bearer ${token}`,
            };
          }
          return config;
        },
        (error) => {
          Promise.reject(error);
        }
      );
    }
    return instance;
  },
};
