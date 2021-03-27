import axios from "axios";
const URL = "";

export function send(method, obj) {
    return new Promise((resolve, reject) => {
        axios.post(URL + "/" + method, obj).then((e) => {
            resolve(e.data);
        })
        .catch((e) => {
            reject(e)
        })
    })
}