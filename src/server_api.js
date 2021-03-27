import axios from "axios";
const URL = "https://back.com.xsph.ru/api/";

export function send(method, obj) {
    const a = new Promise((resolve, reject) => {
        axios.post(URL + method, obj).then((e) => {
            console.log(e)
            resolve(e.data);
        }).catch((e) => console.log(e))
    });
    return a;
}