import React from "react"
const NotFound = () =>{
    return(
        <>
            <div class="d-flex align-items-center justify-content-center vh-100">
            <div class="text-center">
                <h1 class="display-1 fw-bold">404</h1>
                <p class="fs-3"> <span class="text-danger">Chà!</span> Không tìm thấy trang rồi.</p>
                <p class="lead">
                    Trang mà bạn đang tìm không tồn tại 
                  </p>
                <a href="index.html" class="btn btn-primary">Về trang chủ thôi</a>
            </div>
        </div>
        </>
    )
}

export default NotFound;