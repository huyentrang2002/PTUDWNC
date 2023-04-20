import React from "react";
import SearchForm from "./SearchForm";
import CategoriesWidget from "./CategoriesWidget"

const SideBar = ()=>{
    return(
        <div className="pt-4 ps-2">
            <SearchForm/>
            <h1>
                Các chủ đề
            </h1>

            <CategoriesWidget/>
        </div>
    )
}
export default SideBar;