import axios from "axios";
const URL = "https://195.161.62.85:3000/";

export function send(method, obj) {
    const a = new Promise((resolve, reject) => {
        axios.post(URL + method, obj).then((e) => {
            console.log(e)
            resolve(e.data);
        }).catch((e) => console.log(e))
    });
    return a;
}