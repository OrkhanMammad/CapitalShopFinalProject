$(document).on('click', '.delete-card', (function () {
    let productId = this.getAttribute('data-id')
    fetch("/mycard/CardDelete?cardItemId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.basketlist-all').html(data)
        });
}))
$(document).on('click', '.remove-from-wishlist', (function () {
    let productId = this.getAttribute('data-id')
    fetch("/wishlist/RemoveFromWishlist?wishItemId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.wishlist-all').html(data)
        });
}))

$(document).on("click", ".add-address", (function (e) {
    e.preventDefault();
    $('addressContainer').addClass('d-none')
    $('.add-address-form').removeClass('d-none')



}))




$(document).on('click', '.increment-btn', (function () {
    let productId = this.getAttribute('id')
    fetch("/mycard/CardIncreaseCount?cardItemId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.basketlist-all').html(data)
        });
}))

$(document).on('click', '.decrement-btn', (function () {
    let productId = this.getAttribute('id')
    fetch("/mycard/CardDecreaseCount?cardItemId=" + productId)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.basketlist-all').html(data)
        });
}))




$(document).on('click', '.shop-addtowishlist-btn', (function () {
    let productid = this.getAttribute('data-id')
    fetch("/wishlist/addToWishlist?productId=" + productid)

}))

$(document).on('click', '.shop-addtobasket-btn', (function () {
    let productid = this.getAttribute('data-id')
    fetch("/basket/index?productId=" + productid)

}))

$(document).on('click', '.trending-category-selector', (function () {
    let categoryid = this.getAttribute('data-id')
   
    fetch("/home/getTrendingProducts?categoryId=" + categoryid)
        .then(res => {
           return res.text();
        })
        .then(data => {
            console.log(data)
            $('.trending-products-list').html(data)
        });


}))


$(document).on('click', '.pageIndexSelector', (function () {
    let pageindex = this.getAttribute('data-pageIndex')
    let categoryid = this.getAttribute('data-categoryid')
    let producttypeid = this.getAttribute('data-productTypeId')
    let pricerange = this.getAttribute('data-priceRange')
    fetch("/shop/getFilteredProducts?categoryid=" + categoryid + "&productTypeId=" + producttypeid + "&priceRange=" + pricerange + "&pageIndex=" + pageindex)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.products-list-all').html(data)
        });



}))


$(document).on('click', '.category-selector', (function () {

    let categoryid = this.getAttribute('data-categoryid')
    let producttypeid = this.getAttribute('data-productTypeId')
    let pricerange = this.getAttribute('data-priceRange')
    fetch("/shop/getFilteredProducts?categoryid=" + categoryid + "&productTypeId=" + producttypeid + "&priceRange=" + pricerange)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.products-list-all').html(data)
        });

}))


$(document).on('click', '.product-type-selector', (function () {
    let producttypeid = this.getAttribute('data-productTypeId')
    let categoryid = this.getAttribute('data-categoryid')
    let pricerange = this.getAttribute('data-priceRange')
    fetch("/shop/getFilteredProducts?productTypeId=" + producttypeid + "&categoryid=" + categoryid + "&priceRange=" + pricerange)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.products-list-all').html(data)
        });

}))


$(document).on('click', '.price-filter-button', (function () {
    let pricerange = $('#amount').val()
    let producttypeid = this.getAttribute('data-productTypeId')
    let categoryid = this.getAttribute('data-categoryid')
    fetch("/shop/getFilteredProducts?priceRange=" + pricerange + "&productTypeId=" + producttypeid + "&categoryid=" + categoryid)
        .then(res => {
            return res.text();
        })
        .then(data => {
            $('.products-list-all').html(data)
        });

}))



var menubtn = document.getElementById('bottom-menu-btn')
menubtn.addEventListener('click', function () {
    let mobilemenu = document.getElementById('mobile-menu')
    mobilemenu.classList.toggle('mobile-menu-active')
})


var prdinfonavbtns = document.getElementsByClassName('product-info-nav-btn')
for (let prdinfonavbtn of prdinfonavbtns) {
    prdinfonavbtn.addEventListener('click', function () {
        let prevactives = document.getElementsByClassName('active-for-product-info-nav-btn')
        for (let prevactive of prevactives) {
            prevactive.classList.remove('active-for-product-info-nav-btn')
        }
        prdinfonavbtn.classList.add('active-for-product-info-nav-btn')
        let infoboxes = document.getElementsByClassName('product-info-box')
        for (let infobox of infoboxes) {
            if (infobox.getAttribute('id') == prdinfonavbtn.getAttribute('id')) {
                let preactiveboxes = document.getElementsByClassName('active-for-product-info-boxes')
                for (let preactivebox of preactiveboxes) {
                    preactivebox.classList.remove('active-for-product-info-boxes')
                }

                infobox.classList.add('active-for-product-info-boxes')
            }
        }

    })
}


var searchIcons=document.getElementsByClassName('searchIcon')
for(let searchIcon of searchIcons){
    searchIcon.addEventListener('click', function(){
       
        let searchBoxes=document.getElementsByClassName('search-box')
        for(let searchBox of searchBoxes){
            searchBox.classList.toggle('active-for-search')
        }
        
    })
}

var searchCloseIcons=document.getElementsByClassName('close-search-btn')
for(let searchCloseIcon of searchCloseIcons){
    searchCloseIcon.addEventListener('click', function(){
        
        let searchBoxes=document.getElementsByClassName('search-box')
        for(let searchBox of searchBoxes){
            searchBox.classList.toggle('active-for-search')
        }
        
    })
}
