import {Outlet} from 'react-router-dom'
import Navbar from '../Components/Navbar';
import SideBar from '../Components/Sidebar';

const Layout = () =>{
    return(
        <>
            <Navbar/>
            <div className='container-fluid py-3'>
                <div className='row'>
                    <div className='col-9'>
                        <Outlet/>
                    </div>
                    <div className=' col-3 border-start'>
                        <SideBar/>
                    </div>
                </div>
            </div>
        </>
    );
}
export default Layout;