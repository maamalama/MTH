import qs from 'querystring';
import crypto from 'crypto';

export function auth(URL_PARAMS) {
    const urlParams = qs.parse(URL_PARAMS.split("?")[1]);
    const ordered = {};
    Object.keys(urlParams).sort().forEach((key) => {
        if (key.slice(0, 3) === 'vk_') {
            ordered[key] = urlParams[key];
        }
    });
    
    const stringParams = qs.stringify(ordered)
    const paramsHash = crypto
        .createHmac('sha256', "ycCuLsYykCDZrHA5S8KI")
        .update(stringParams)
        .digest()
        .toString('base64')
        .replace(/\+/g, '-')
        .replace(/\//g, '_')
        .replace(/=$/, '');
    return paramsHash === urlParams.sign ? urlParams.vk_user_id : false;
}