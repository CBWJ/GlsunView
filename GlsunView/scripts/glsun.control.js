; (function () {
    //标签页类
    function TabPage(title, url){
        this.title = title;
        this.url = url;
        this.headId = "";
        this.contentId = "";
    }

    //标签控件类
    function TabControl() {
        //点击关闭标记
        var clickClose = false;
        this.tabPages = {};
        this.count = 0;
        this.selector = "";
        this.showUrl = "";
        this.settings = {onTabClose:null};

        this.init = function(selector,options){
            this.selector = selector;
            $.extend(this.settings, options);
            $(selector).append('<div class="tab-header clearfix"><div id="scroll-header-left" class="less-header"><i class="fa fa-chevron-circle-left"></i></div><ul></ul><div id="scroll-header-right" class="less-header"><i class="fa fa-chevron-circle-right"></i></div> </div><div class="tab-content"></div>');
            var self = this;
            //右侧
            $("#scroll-header-right").on("click",function(){
                if($(this).hasClass("more-header")){
                    self.moveAllTabHeader(50, "left");
                    var ulWidth = self.getShowTabPageHeaderWidth();
                    var left = self.getTabPagePosition(self.getLastTabPage()).left;
                    var width = self.getTabPageHeaderWidth(self.getLastTabPage());
                    if(left + width < ulWidth){
                        self.moveAllTabHeader(ulWidth-left-width, "right");
                    }
                    self.showScrollHeader();
                }
            })
            //左侧
            $("#scroll-header-left").on("click",function(){
                if($(this).hasClass("more-header")){
                    self.moveAllTabHeader(50, "right");
                    //左边
                    var left = self.getTabPagePosition(self.getFirstTabPage()).left;
                    if(left > 0){
                        self.moveAllTabHeader(left, "left");
                    }
                    self.showScrollHeader();
                }
            })
            $(window).resize(function(){
                self.showScrollHeader();
            });
            return this;
        }

        this.add = function(title, url){
            var tab = this.tabPages[url]
            if(tab){
                this.hideAllTabContent();
                this.showTabPage(tab);
                return this;
            }
            tab = new TabPage(title, url);
            var id = getNextId();
            tab.headId = "head" + id;
            tab.contentId = "content" + id;
            
            this.tabPages[url] = tab;
            this.count++;
            this.addTabPage(tab);
            this.hideAllTabContent();
            this.showTabPage(tab);
            this.widthAdjustment();
            return this;
        }

        this.remove = function(url){
            var tab = this.tabPages[url];
            if(tab){
                this.closeTabPage(tab);
            }
            return this;
        }

        this.addTabPage = function (tabPage){
            //构建标题和内容DIV插入到相应位置
            var header = document.createElement("li");
            var span = document.createElement("span");
            var link = document.createElement("a");
            var close = document.createElement("i");
            var placeholder = document.createElement("div");
    
            span.innerText = tabPage.title;
            close.className = "fa fa-close fa-fw";
            link.href = "#";
            header.id = tabPage.headId;
            var self = this;
            $(close).on("click", function (e) {
                clickClose = true;
                self.headerCloseClick(this);
                e.stopPropagation();
            });
            placeholder.className = "blank-placeholder fa-fw";
            link.appendChild(span);
            link.appendChild(close);
            link.appendChild(placeholder);
            header.appendChild(link);
    
            $(header).on("click", function(e){
                    self.headerClick(header);
                    e.stopPropagation();
                });
            var widths = this.getAllTabPageHeaderWidth();
            
            if(this.count > 0 && widths != 0){
                //左边
                var left = this.getTabPagePosition(this.getFirstTabPage()).left;
                widths += left;
            }
            if (widths != 0) {
                $(header).css("left", widths);
            }
            $(this.selector + " .tab-header UL")[0].appendChild(header);
    
            //添加内容
            var frame = document.createElement("iframe");
            frame.src = tabPage.url;
            frame.className = "tab-content-frame";
            var content = document.createElement("div");
            content.id = tabPage.contentId;
            content.style.display = "block";
            content.appendChild(frame);
            $(this.selector + " .tab-content")[0].appendChild(content);        
        }

        //所有标题的宽度
        this.getAllTabPageHeaderWidth = function (){
            var len = 0;
            $(this.selector + " .tab-header ul li").each(function(i){
                var w = $(this).outerWidth();
                len += w;
            });
            return len;
        }
        this.showTabPage = function (tabPage) {
            if (isShown(tabPage)) return;
            $("#" + tabPage.headId).addClass("selected");
            $("#" + tabPage.contentId).fadeIn();
            this.showUrl = tabPage.url;
            //左边
            var left = this.getTabPagePosition(tabPage).left;
            if(left < 0){
                this.moveAllTabHeader(0-left + 2, "right");
            }else{
                var header = $("#"+tabPage.headId);
                var ulWidth = this.getShowTabPageHeaderWidth();
                var w = header.innerWidth();
                /* if(left >= ulWidth){
                    this.moveAllTabHeader(left - ulWidth + w, "left");
                }
                else  */
                if(left + w > ulWidth){
                    this.moveAllTabHeader(left + w - ulWidth + 2, "left");
                }
            }
            this.showScrollHeader();
        }
        //是否显示
        function isShown(tabPage) {
            var val = $("#" + tabPage.contentId).css("display");
            if (val == "block") {
                return true;
            }
            return false;
        }
        //隐藏所有
        this.hideAllTabContent = function (){
            $(this.selector + " .tab-header UL LI").each(function(){
                $(this).removeClass("selected");
            });
            $(this.selector + " .tab-content div").each(function(){
                $(this).hide();
            });
        }
        //通过标题ID获取标签页
        this.getTabPageByHeadId = function (headId){
            for(var url in this.tabPages){
                if(this.tabPages[url].headId == headId)
                    return this.tabPages[url];
            }
            return null;
        }

        this.headerClick = function(header){
            var tabPage = this.getTabPageByHeadId(header.id);
            if(this.showUrl == tabPage.url) return;
            this.hideAllTabContent();
            this.showTabPage(tabPage);
        }

        this.headerCloseClick = function (close){
            var id = $(close).parent("a").parent("li")[0].id;
            var tabClose = this.getTabPageByHeadId(id);
            this.closeTabPage(tabClose);
            if(this.settings.onTabClose){
                this.settings.onTabClose.call(this, tabClose);
            }
        }

        this.closeTabPage = function (tabPage){
            var header = $("#"+tabPage.headId);
            var left = header.css("left");
            
            var width = header.outerWidth();
            var innerW = header.innerWidth();
            $("#"+tabPage.headId).remove();
            $("#"+tabPage.contentId).remove();
            var move = false;
            var prev,next;
            for(var url in this.tabPages){
                if(move){
                    var tmp = $("#"+this.tabPages[url].headId).css("left").slice(0,-2);
                    var newLeft = tmp -innerW;
                    /* if(newLeft < 0)
                        newLeft = 0; */
                    $("#"+this.tabPages[url].headId).css("left",newLeft);
                    if(next == null)
                        next = this.tabPages[url];
                }
                if(url == tabPage.url){
                    move = true;
                }
                if(!move){
                    prev = this.tabPages[url];
                }
            }
            delete this.tabPages[tabPage.url];
            this.count--;
            if (clickClose) {
                if (next) {
                    this.showTabPage(next);
                }
                else if (prev) {
                    this.showTabPage(prev);
                }
                clickClose = false;
            }
        }

        this.getTabPageByIndex = function(index){
            if(index >= this.count) return null;
            var i = 0;
            for(var url in this.tabPages){
                if(i == index)  return this.tabPages[url];
                i++;
            }
        }

        this.getFirstTabPage = function(){
            return this.getTabPageByIndex(0);
        }
        this.getLastTabPage = function(){
            return this.getTabPageByIndex(this.count - 1);
        }
        this.getTabPagePosition = function(tabPage){
            if(tabPage == null) return null;
            return $("#"+tabPage.headId).position()
        }
        this.getTabPageHeaderWidth = function(tabPage){
            if(tabPage == null) return null;
            return $("#"+tabPage.headId).outerWidth();
        }
        //宽度调整
        this.widthAdjustment = function () {
            //左边
            var leftFirst = this.getTabPagePosition(this.getFirstTabPage()).left;
            for (var i = 0; i < this.count; ++i) {
                var tab = this.getTabPageByIndex(i);
                var $tab = $("#" + tab.headId);
                var left = leftFirst;
                if (i == 0) {
                    $tab.css("left", left);
                }
                else {
                    var preTab = this.getTabPageByIndex(i - 1);
                    var $preTab = $("#" + preTab.headId);
                    left = $preTab.css("left");
                    width = this.getTabPageHeaderWidth(preTab);
                    var netLeft = Number.parseInt(left);
                    $tab.css("left", netLeft + width);
                }
            }
        }
        //标题UL的宽度
        this.getShowTabPageHeaderWidth = function(){
            return $($(".tab-header ul")[0]).outerWidth();
        }
        //更新标题
        this.updateTitle = function(url, title){
            var tabPage = this.tabPages[url];
            if (tabPage) {
                var li = $("#" + tabPage.headId);
                var span = li.children("a").children("span");
                span.text(title);
                this.widthAdjustment();
            }
        }
        //显示标题滚动按钮
        this.showScrollHeader = function () {
            if (this.count == 0) return;
            //左边
            var firstLeft = this.getTabPagePosition(this.getFirstTabPage()).left;
            if(firstLeft < 0){
                $("#scroll-header-left").removeClass("less-header").addClass("more-header");
            }else{
                $("#scroll-header-left").removeClass("more-header").addClass("less-header");
            }
            //var ulWidth = $($(".tab-header ul")[0]).outerWidth();
            var ulWidth = this.getShowTabPageHeaderWidth();
            var LastLeft = this.getTabPagePosition(this.getLastTabPage()).left;
            var lastWidth = this.getTabPageHeaderWidth(this.getLastTabPage());
            if(LastLeft + lastWidth > ulWidth){
                $("#scroll-header-right").addClass("more-header").removeClass("less-header");
            }else{
                $("#scroll-header-right").addClass("less-header").removeClass("more-header");
            }
        }

        //左右移动标题
        this.moveTabHeader = function(tabPage, offset, orientation){
            if(!tabPage) return;
            var p = $("#"+tabPage.headId).position();
            var headerleft = p.left;
            if(orientation === "left"){
                headerleft -= offset;
            }else if(orientation === "right"){
                headerleft += offset;
            }
            var header = document.getElementById(tabPage.headId);
            $(header).css("left", headerleft);
        }
        this.moveAllTabHeader = function (offset, orientation){
            for(var url in this.tabPages){
                this.moveTabHeader(this.tabPages[url], offset, orientation);
            }
        }
    }

    //状态信息类
    function Status(){
        this.selector = "";
        this.init = function(selector){
            this.selector = selector;
        };
        this.append = function(content, type){
            var span = document.createElement("span");
            span.innerText = content;
            span.className = "msg-"+type;
            var s = $(this.selector);
            s.append(span);
        };
        this.appendNormal = function(content){
            this.append(content, "normal");
        }
        this.appendImportant = function(content){
            this.append(content, "important");
        }
        this.appendWarnning = function(content){
            this.append(content, "warnning");
        }
        this.appendError = function(content){
            this.append(content, "error");
        }

        this.appendLine = function(content, type){
            this.append(content, type);
            $(this.selector).append("<br>");
        };
        this.appendLineForNormal = function(content){
            this.appendLine(content, "normal");
        }
        this.appendLineForImportant = function(content){
            this.appendLine(content, "important");
        }
        this.appendLineForWarnning = function(content){
            this.appendLine(content, "warnning");
        }
        this.appendLineForError = function(content){
            this.appendLine(content, "error");
        }

        this.clear = function(){
            $(this.selector).children().remove();
        }
    }
    //工具方法
    var next = 100;
    function getNextId(){
        return next++;
    };

    //命名空间
    window.glsun = {};
    glsun.tab = new TabControl();
    glsun.status = new Status();
}());