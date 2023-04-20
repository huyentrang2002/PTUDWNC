import React, { useState, useEffect } from "react";
import Table from "react-bootstrap/Table";
import { Link, useParams } from "react-router-dom";
import { getPostsFilter } from "../../../Services/BlogRepository";
import Loading from "../../../Components/Loading";
import PostFilterPane from '../../../Components/Admin/PostFilterPane';

import { isInteger } from '../../../Utils/Ultils'
import { useSelector } from 'react-redux';

const Posts = () => {
    const [postList, setPostsList] = useState([]),
            [isVisibleLoading, setIsVisibleLoading] = useState(true),
            postFilter= useSelector(state => state.postFilter);

    let {id} =useParams(),
        p=1,ps=10
    
    //let k = '', p = 1, ps = 10

    // useEffect(() => {
    //     document.title = 'Danh sách bài viết'

    //     getPosts(k, p, ps).then(data => {
    //         if (data)
    //             setPostList(data.items)
    //         else
    //             setPostList([])
    //         setIsVisibleLoading(false)
    //     })
    // }, [k, p, ps])

    useEffect(()=>{
        document.title = 'Danh sách bài viết';  
        getPostsFilter(postFilter.keyword,  
            postFilter.authorId,  
            postFilter.categoryId,  
            postFilter.year,  
            postFilter.month,  
            ps, p).then(data => {  
                if (data)  
                    setPostsList(data.items); 
                else 
                    setPostsList([]); 
                setIsVisibleLoading(false); 
            }); 
    },[ 
        postFilter.keyword,  
        postFilter.authorId,  
        postFilter.categoryId,  
        postFilter.year,  
        postFilter.month,  
        p, ps 
    ]);  

    return (
        <>
            <h1>Danh sách bài bài viết</h1>
            <PostFilterPane />
            {isVisibleLoading ? <Loading /> :
                <Table striped responsive bordered>
                    <thead>
                        <tr>
                            <th>Tiêu đề</th>
                            <th>Tác giả</th>
                            <th>Chủ đề</th>
                            <th>Xuất bản</th>
                        </tr>
                    </thead>

                    <tbody>
                        {postList.length > 0 ? postList.map((item, index) =>
                            <tr key={index}>
                                <td>
                                    <Link
                                        to={`/admin/posts/edit/${item.id}`}
                                        className='text-bold'>
                                        {item.title}
                                    </Link>
                                    <p className="text-muted">{item.shortDescription}</p>
                                </td>
                                <td>{item.author.fullName}</td>
                                <td>{item.category.name}</td>
                                <td>{item.published ? 'Co' : 'Khong'}</td>
                            </tr>
                        ) :
                            <tr>
                                <td colSpan={4}>
                                    <h4 className="text-danger text-center">Không tìm thấy bài viết nào</h4>
                                </td>
                            </tr>}
                    </tbody>
                </Table>
            }
        </>
    );
}
export default Posts;