import axios from "axios";
import { get_api } from "./Method";

// export async function getCategories() {

//     const axiosClient = axios.create({
//         baseURL: 'https://localhost:7007/api/',
//         headers: {
//             'Access-Control-Allow-Origin': '*',
//             'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE,PATCH,OPTIONS',
//             'Content-Type': 'application/json',
//         },
//         withCredentials: false
//     });

//     const params = {
//         PageSize: 10,
//         PageNumber: 1
//     }
//     try {
//         const response = await
//             axiosClient.get('categories', { params })

//         const data = response.data
//         if (data.isSuccess)
//             return data.result
//         else
//             return null
//     } catch (error) {
//         console.log('Error', error.message);
//         return null
//     }
// }

export function getCategories(){
    return get_api(`https://localhost:7007/api/categories`)
}