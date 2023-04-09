import axios from "axios";

export async function GetFilteredPosts()
   // keyword = '', pageSize = 10, pageNumber = 1,
   // sortColumn = '', sortOrder = '')
     {
    try {
        const response = await
            axios.get(`https://localhost:7007/api/posts/get-posts-filter?PageSize=10&PageNumber=1`);

        const data = response.data

        if (data.isSuccess) {
            return data.result
        }
        else
            return null
    } catch (error) {
        console.log('Error', error.message);
        return null
    }
}