//定义一个匿名方法并执行
(function(){
    function SimpleMenu(selector){
        this.selector = selector;
    }
    SimpleMenu.prototype.init = function(clickCallback){
        var self = this;
        window.oncontextmenu = function(e){
            //取消默认的浏览器自带右键 很重要！！
            e.preventDefault();
            var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
            var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;
            var menu = document.querySelector(self.selector);
            menu.style.left = e.clientX + scrollLeft + "px";
            menu.style.top = e.clientY + scrollTop + "px";
            menu.style.display = "block";
        }
        window.onclick = function (e) { 
            var menu = document.querySelector(self.selector);
            menu.style.display = "none";
            }
        
        var items = document.getElementsByClassName("menu-item");
        for(var i = 0; i < items.length; ++i){
            var item = items[i];
            item.onclick = function (e) {  
                //console.log(this.textContent);
                if(clickCallback != null && typeof clickCallback == "function"){
                    clickCallback.call(this);
                }
            }
        }
    }
    window.SimpleMenu = SimpleMenu;
}())