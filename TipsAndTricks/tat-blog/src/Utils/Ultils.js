export function isEmptyOrSpaces(str){
    return str === null || (typeof str ==='string' 
    && str.match(/^ *$/) !== null)
}

export function isInteger(str) { 
    return Number.isInteger(Number(str)) && Number(str) >= 0;  
}

export function decode(str) { 
    let txt = new DOMParser().parseFromString(str, "text/html");  
    return txt.documentElement.textContent
}