/*
鼠标悬浮提示文本
*/
(function($){
    if(!$) return;
    $.fn.tooltip = function(options){
        var settings = {
            text: ''
        };
        $.extend(settings, options);
        //this是一个jQuery对象 
        var $elem = this;
        var span = document.createElement('span');
        span.innerText = settings.text;
        var $span = $(span);
        $span.addClass('tooltip-text');                    
        $span.appendTo($elem.parent());
        

        var bLeave = false;
        var bCount = false;
        var count = 0;
        var tick = null;
        var point = {x:0,y:0};

        $elem.on('mouseover', function(e){
            var parentOffset = $elem.parent().offset();
            console.log(parentOffset);

            count = 0;
            bLeave = false;
            //进入
            if(!bCount){
                bCount = true;
                tick = setInterval(function(){
                    count++;
                    if(count > 200){
                        clearInterval(tick);
                        /*定位上下文问题 */
                        $span.css('left', point.x - parentOffset.left);
                        $span.css('top', point.y - parentOffset.top + 20 );
                        $span.fadeIn(100);
                        bCount = false;
                    }
                    if(bLeave){
                        clearInterval(tick);
                        bCount = false;
                    }
                }, 1);
            }

        }).on('mouseleave',function(e){
            bLeave = true;
            $span.fadeOut(200);
        }).on('mousemove',function(e){
            point.x = e.clientX;
            point.y = e.clientY;
        });
    };
}(jQuery));