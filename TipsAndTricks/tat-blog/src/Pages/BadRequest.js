import { Link } from 'react-router-dom'
import { useQuery } from '../Utils/Ultils'

const BadRequest = () => {
    // let query = useQuery(),
    //     redirectTo = query.get('rredirectTo') ?? '/'

    return (
        <>
            <div class="d-flex align-items-center justify-content-center vh-100">
                <div class="text-center">
                    <h1 class="display-1 fw-bold">404</h1>
                    <p class="fs-3"> <span class="text-danger">Chà!</span> Yêu cầu không hợp lệ.</p>
                    <p class="lead">
                        Có vẻ tham số trong URL của bạn không đúng yêu cầu
                    </p>
                    <a href="index.html" class="btn btn-primary">Về trang chủ thôi</a>
                </div>
            </div>
        </>
    )
}
export default BadRequest;