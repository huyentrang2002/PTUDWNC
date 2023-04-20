import axios from "axios";
import PostFilterPane from "../Components/Admin/PostFilterPane";
import { get_api, post_api } from "./Method";

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

export function getPosts(keyword = '',
    pageSize = 10,
    pageNumber = 1,
    sortColumn = '',
    sortOrder = '') {
    return get_api(`https://localhost:7007/api/posts?keyword=${keyword}&PageSize=${pageSize}&PageNumber=${pageNumber}&SortColumn=${sortColumn}&SortOder=${sortOrder}`)
}

export function getAuthors(name = '',
    pageSize = 10,
    pageNumber = 1,
    sortColumn = '',
    sortOrder = '') {
    return get_api(`https://localhost:7007/api/authors?name=${name}&PageSize=${pageSize}&PageNumber=${pageNumber}&SortColumn=${sortColumn}&SortOder=${sortOrder}`)
}

export function getFilter() {
    return get_api('https://localhost:7007/api/posts/get-filter/')
}

export function getPostsFilter(keyword = '',
    authorId = '', categoryId = '',
    year = '', month = '',
    pageSize = 10, pageNumber = 1,
    sortColumn = '', sortOrder = '') {
    let url = new URL('https://localhost:7007/api/posts/get-posts-filter');

    keyword !== '' && url.searchParams.append('Keyword', keyword);
    authorId !== '' && url.searchParams.append('AuthorId', authorId);
    categoryId !== '' && url.searchParams.append('CategoryId', categoryId);
    year !== '' && url.searchParams.append('Year', year);
    month !== '' && url.searchParams.append('Month', month);
    sortColumn !== '' && url.searchParams.append('SortColumn', sortColumn);
    sortOrder !== '' && url.searchParams.append('SortOrder', sortOrder);
    url.searchParams.append('PageSize', pageSize);
    url.searchParams.append('PageNumber', pageNumber);

    return get_api(url.href);
}

export async function getPostById(id = 0) {
    if (id > 0)
        return get_api(`https://localhost:7085/api/posts/${id}`); return null;
}

export function addOrUpdatePost(formData) {
    return post_api('https://localhost:7085/api/posts', formData);
}

export default PostFilterPane;
