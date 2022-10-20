import axios from "axios";
import { config } from "../config.js"

const request = axios.create({
    baseURL: config.api
});
request.interceptors.request.use(async function (config) {
    try {
            console.log(config.api)
            config.headers.Tenant = "00000000-0000-0000-0000-000000000000";

        return config;
    } catch (error) {
        return Promise.reject(error);
    }
});
export default request;