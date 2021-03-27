import axios from "axios";
const URL = "https://api-waste.hhos.ru/mth/";

export function send(method, obj) {
    return new Promise((resolve, reject) => {
        axios.post(URL, JSON.stringify({ type: method, obj })).then((e) => {
            resolve(e.data);
        })
        .catch((e) => {
            reject(e)
        })
    })
}