//定义一个函数立即执行
(function(){
    var Route = function(canvas){
        //获取绘制上下文对象
        var c = canvas.getContext("2d");
        var padding = 5;
        //尺寸参数
        var titleWidth = 100;
        var pinWidth = 70;
        var boxWidth = 130;
        var boxHeight = 180;
        var boxPadding = 10;
        var textHeight = 35;
        var portHeiht = 20;
        var portWidth = 40;
        var textBoxPadding = 3;
        var textBoxHeight = 20;
        var valeuOffset = 2;
        var valueFontHeight = 20;
        //样式参数
        var backroundColor = "#eee";    //画布背景色  
        var linkColor = "#0472c0";      //链路颜色
        var titleFont = "24px 微软雅黑";
        var textFont = "14px 微软雅黑";
        var portFont = "16px sans-serif";
        var valueFont = "20px Calibri";        
        //计算参数
        var boxA_X = titleWidth + pinWidth, boxA_Y = textHeight;
        var boxB_X = canvas.width - titleWidth - pinWidth - boxWidth, boxB_Y = textHeight;
        var boxLevelInterval = boxWidth - 2 * boxPadding - 2 * portWidth;
        var boxVeticalInterval = boxHeight - 2 * boxPadding - 6 * portHeiht;
        var xx, yy;
        //两端连线变量
        var lineStartX = boxA_X + boxWidth - boxPadding;
        var lineEndX = boxB_X + boxPadding;
        var lineY1 = boxA_Y + boxPadding + portHeiht / 2,
            lineY2 = lineY1 + portHeiht * 2,
            lineY3 = lineY2 + portHeiht + boxVeticalInterval,
            lineY4 = lineY3 + portHeiht * 2;
        //A框开关
        var boxA_TX_X = boxA_X + boxPadding + portWidth, boxA_TX_Y = boxA_Y + boxPadding + portHeiht * 3 / 2;
        var boxA_T1_X = boxA_TX_X + boxLevelInterval, boxA_T1_Y = boxA_Y + boxPadding + portHeiht / 2;
        var boxA_T2_X = boxA_T1_X, boxA_T2_Y = boxA_T1_Y + 2 * portHeiht;
        var boxA_RX_X = boxA_TX_X, boxA_RX_Y = boxA_TX_Y + boxVeticalInterval + 3 * portHeiht;
        var boxA_R1_X = boxA_T1_X, boxA_R1_Y = boxA_T2_Y + boxVeticalInterval + portHeiht;
        var boxA_R2_X = boxA_T1_X, boxA_R2_Y = boxA_R1_Y + portHeiht * 2;
        //B框开关
        var boxB_R1_X = boxB_X + boxPadding + portWidth, boxB_R1_Y = boxA_T1_Y;
        var boxB_R2_X = boxB_R1_X, boxB_R2_Y = boxA_T2_Y;
        var boxB_RX_X = boxB_R1_X + boxLevelInterval, boxB_RX_Y = boxA_TX_Y;
        var boxB_T1_X = boxB_R1_X, boxB_T1_Y = boxA_R1_Y;
        var boxB_T2_X = boxB_R1_X, boxB_T2_Y = boxA_R2_Y;
        var boxB_TX_X = boxB_RX_X, boxB_TX_Y = boxA_RX_Y;
        //左右标题
        var leftTitleText = "A端", rightTitleText = "B端";
        var leftTitleRect = rightTitleRect = null;
        //框顶文本
        var leftBoxTopText = "A端机框IP-盘号",rightBoxTopText = "A端机框IP-盘号";
        var leftBoxTopRect = rightBoxTopRect = null;
        //框低文本
        var leftBoxBottomText = rightBoxBottomText = "OLP";
        var leftBoxBottomRect = rightBoxBottomRect = null;
        //中间文本
        var centerTitle = leftTitleText + "-" + rightTitleText;
        var centerTitleRect = null;
        //显示值
        var leftTX = leftT1 = leftT2 = leftRX = leftR1 = leftR2 = 0;
        var rightTX = rightT1 = rightT2 = rightRX = rightR1 = rightR2 = 0;
        var diff_T1_R1 = diff_T2_R2 = diff_R1_T1 = diff_R2_T2 = 0;
        //清空画布
        function clearCanvas(){
            c.clearRect(0, 0, canvas.width, canvas.height);
        }
        //绘制静态内容
        function drawStatic(){
            clearCanvas();
            c.fillStyle = backroundColor;
            c.fillRect(0, 0, canvas.width, canvas.height); 
            //左边框
            drawBox(boxA_X, boxA_Y, boxWidth, boxHeight);
            //发
            xx = boxA_X + boxPadding;
            yy = boxA_Y + boxPadding + portHeiht;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "TX");
            xx = boxA_X + boxPadding + boxLevelInterval + portWidth;
            yy = boxA_Y + boxPadding;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "T1");
            yy = boxA_Y + boxPadding + portHeiht * 2;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "T2");
            //收
            xx = boxA_X + boxPadding;
            yy = boxA_Y + boxVeticalInterval + boxPadding + portHeiht * 4;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "RX");
            xx = boxA_X + boxPadding + boxLevelInterval + portWidth;
            yy = boxA_Y + boxVeticalInterval + boxPadding + portHeiht * 3;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "R1");
            yy = boxA_Y + boxVeticalInterval + boxPadding + portHeiht * 5;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "R2");
            //右边框
            drawBox(boxB_X, boxB_Y, boxWidth, boxHeight);
            //收
            xx = boxB_X + boxPadding;     
            yy = boxB_Y + boxPadding;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "R1");
            yy = boxB_Y + boxPadding + portHeiht * 2;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "R2");
            xx = boxB_X + boxPadding + boxLevelInterval + portWidth;
            yy = boxB_Y + boxPadding + portHeiht;
            drawPortBox(xx, yy, portWidth, portHeiht, "#09b150", "RX");
            xx = boxB_X + boxPadding;     
            yy = boxB_Y + boxVeticalInterval + boxPadding + portHeiht * 3;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "T1");
            yy = boxB_Y + boxVeticalInterval + boxPadding + portHeiht * 5;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "T2");
            xx = boxB_X + boxPadding + boxLevelInterval + portWidth;
            yy = boxB_Y + boxVeticalInterval + boxPadding + portHeiht * 4;
            drawPortBox(xx, yy, portWidth, portHeiht, "#ffc100", "TX");
            //两端连线
            drawPortLine(lineStartX, lineY1, lineEndX, lineY1, "#888");
            drawPortLine(lineStartX, lineY2, lineEndX, lineY2, "#888");
            drawPortLine(lineStartX, lineY3, lineEndX, lineY3, "#888");
            drawPortLine(lineStartX, lineY4, lineEndX, lineY4, "#888");
            //公共端连线
            drawPortLine(titleWidth, boxA_TX_Y, titleWidth + pinWidth + boxPadding, boxA_TX_Y, linkColor);
            drawPortLine(titleWidth, boxA_RX_Y, titleWidth + pinWidth + boxPadding, boxA_RX_Y, linkColor);
            drawPortLine(boxB_RX_X + portWidth, boxA_TX_Y, boxB_RX_X + portWidth + pinWidth + boxPadding, boxA_TX_Y, linkColor);
            drawPortLine(boxB_RX_X + portWidth, boxA_RX_Y, boxB_RX_X + portWidth + pinWidth + boxPadding, boxA_RX_Y, linkColor);
            //左右标题
            drawTitleLeft(leftTitleText);
            drawTitleRight(rightTitleText);
            //框顶文本
            drawLeftBoxTopText(leftBoxTopText);
            drawRightBoxTopText(rightBoxTopText);
            //框底文本
            drawLeftBoxBottomText(leftBoxBottomText);
            drawRightBoxBottomText(rightBoxBottomText);
            //路由名称
            drawCenterText(centerTitle);
            //显示值
            drawLeftTX(leftTX);
            drawLeftT1(leftT1);
            drawLeftT2(leftT2);
            drawLeftRX(leftRX);
            drawLeftR1(leftR1);
            drawLeftR2(leftR2);

            drawRightTX(rightTX);
            drawRightT1(rightT1);
            drawRightT2(rightT2);
            drawRightRX(rightRX);
            drawRightR1(rightR1);
            drawRightR2(rightR2);

        }
        //绘制大框
        function drawBox(x, y, w, h){
            c.beginPath();
            //框
            c.rect(x, y, w, h);        
            c.stroke();
            //填充
            //线性渐变
            var boxGradient = c.createLinearGradient(x, y, x + w, y + h);
            boxGradient.addColorStop(0.0, "#e6ebd5");
            boxGradient.addColorStop(1.0, "#fff");
            c.fillStyle = boxGradient;
            c.fillRect(x, y, w, h);
        }
        //绘制端口框
        function drawPortBox(x, y, w, h, style, text){
            c.save();
            c.beginPath();
            c.fillStyle = style;
            c.rect(x, y, w, h);
            c.closePath();
            c.stroke();
            c.fill();
            c.beginPath();
            c.font = portFont;
            c.textAlign = "center";
            c.textBaseline = "middle";
            c.fillStyle = "#000";
            c.fillText(text, x + ( w / 2), y + (h / 2));
            c.closePath();
            c.restore();
        }
        //绘制端口连线
        function drawPortLine(x1, y1, x2, y2, style){
            c.save();
            c.lineWidth = 5;
            c.strokeStyle = style;
            c.lineCap = "round";
            c.beginPath();
            c.moveTo(x1, y1);
            c.lineTo(x2, y2);
            c.stroke();
            c.closePath();
            c.restore();
        }
        //绘制左右的标题
        function drawTitle(text, titleX, titelY, minX, maxWidth, drawLeft){
            c.save();
            //清空之前的数据
            if(drawLeft){
                if(leftTitleRect) clearTitleRect(leftTitleRect);
            }
            else{
                if(rightTitleRect) clearTitleRect(rightTitleRect);
            }
            c.font = titleFont;
            c.textAlign = "center";
            c.textBaseline = "middle";
            c.strokeStyle = "#bbb";
            c.fillStyle = "#fff";
            c.lineWidth = 1;
            var size = c.measureText(text);
            //字数多调整X
            if(size.width > titleWidth - 10){
                titleX = minX + maxWidth / 2;
            }
            //框
            var x = titleX - size.width / 2 - 5,
                y = canvas.height / 2 - 17,
                w = size.width + 10,
                h = 34;
            //保证矩形框在画布内
            if(x < minX){
                if(drawLeft){            
                    titleX -= x - 2;
                    x = minX + 2;
                }else{
                    titleX += canvas.width - (maxWidth + x);
                    x = canvas.width - maxWidth + 2;
                }
            }
            //保证一定的宽度
            if(w > maxWidth - 4){
                var more = w - (maxWidth - 4);
                titleX -= more / 2;
                w = maxWidth - 4;
            }
            c.strokeRect(x, y, w, h);        
            c.fillRect(x, y, w, h);
            c.fillStyle = "#000";
            c.fillText(text, titleX, titelY, w);
            c.restore();
            if(drawLeft){
                leftTitleRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }else{
                rightTitleRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }
        }
        //左标题
        function drawTitleLeft(text){
            leftTitleText = text;
            drawTitle(text, titleWidth / 2, canvas.height / 2, 0, titleWidth + pinWidth, true);
        }
        //右标题
        function drawTitleRight(text){
            rightTitleText = text;
            drawTitle(text, canvas.width - titleWidth / 2, canvas.height / 2, boxB_X + boxWidth, titleWidth + pinWidth, false);
        }
        //清除矩形框
        function clearTitleRect(rect){
            c.fillStyle = backroundColor;
            c.fillRect(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2);
        }
        //绘制框顶文本
        function drawBoxTopText(text, textX, textY, drawLeft){
            c.save();
            //清空之前的数据
            if(drawLeft){
                if(leftBoxTopRect) clearTitleRect(leftBoxTopRect);
            }
            else{
                if(rightBoxTopRect) clearTitleRect(rightBoxTopRect);
            }
            c.font = textFont;
            c.textAlign = "center";
            c.textBaseline = "middle";
            c.strokeStyle = "#bbb";
            c.lineWidth = 1;
            var size = c.measureText(text);
            x = textX - size.width / 2 - textBoxPadding;
            y = textY - textBoxHeight / 2;
            w = size.width + 2 * padding;
            h = textBoxHeight;
            c.fillStyle = "#c5d7a1";
            c.strokeRect(x, y, w, h);        
            c.fillRect(x, y, w, h);
            c.fillStyle = "#000";
            c.fillText(text, textX, textY);
            c.restore();
            if(drawLeft){
                leftBoxTopRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }else{
                rightBoxTopRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }
        }
        //
        function drawLeftBoxTopText(text){
            leftBoxTopText = text;
            drawBoxTopText(text, boxA_X + boxWidth / 2, leftBoxTopText_Y = textHeight / 2, true);
        }
        function drawRightBoxTopText(text){
            rightBoxTopText = text;
            drawBoxTopText(text, boxB_X + boxWidth / 2, leftBoxTopText_Y = textHeight / 2, false);
        }
        //绘制框底文本
        function drawBoxBottomText(text, textX, textY, drawLeft){
            c.save();
            //清空之前的数据
            if(drawLeft){
                if(leftBoxBottomRect) clearTitleRect(leftBoxBottomRect);
            }
            else{
                if(rightBoxBottomRect) clearTitleRect(rightBoxBottomRect);
            }
            c.font = textFont;
            c.textAlign = "center";
            c.textBaseline = "middle";
            c.strokeStyle = "#bbb";
            c.lineWidth = 1;
            var size = c.measureText(text);
            x = textX - size.width / 2 - textBoxPadding;
            y = textY - textBoxHeight / 2;
            w = size.width + 2 * textBoxPadding;
            h = textBoxHeight;
            c.strokeRect(x, y, w, h);        
            //线性渐变
            var bgfade = c.createLinearGradient(textX, textY - h / 2, textX, textY + h / 2);
            bgfade.addColorStop(0.0, "#c5d7a1");
            bgfade.addColorStop(0.5, "#fff");
            bgfade.addColorStop(1.0, "#c5d7a1");
            c.fillStyle = bgfade;
            c.fillRect(x, y, w, h);
            c.fillStyle = "#000";
            c.fillText(text, textX, textY);
            c.restore();
            if(drawLeft){
                leftBoxBottomRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }else{
                rightBoxBottomRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
            }
        }
        function drawLeftBoxBottomText(text){
            leftBoxBottomText = text;
            drawBoxBottomText(text, boxA_X + boxWidth / 2, canvas.height - textHeight / 2, true);
        }
        function drawRightBoxBottomText(text){
            rightBoxBottomText = text;
            drawBoxBottomText(text, boxB_X + boxWidth / 2, canvas.height - textHeight / 2, false);
        }
        //绘制中间文本
        function drawCenterText(text){
            c.save();
            centerTitle = text;
            if(centerTitleRect) clearTitleRect(centerTitleRect);
            c.font = textFont;
            c.textAlign = "center";
            c.textBaseline = "middle";
            c.strokeStyle = "#bbb";
            c.lineWidth = 1;
            var textX = boxB_X - (boxB_X - boxA_X - boxWidth) / 2, textY = textHeight / 2;
            var size = c.measureText(text);
            x = textX - size.width / 2 - textBoxPadding;
            y = textY - textBoxHeight / 2;
            w = size.width + 2 * textBoxPadding;
            h = textBoxHeight;
            c.fillStyle = "#ddd7e5";
            c.strokeRect(x, y, w, h);        
            c.fillRect(x, y, w, h);
            c.fillStyle = "#000";
            c.fillText(text, textX, textY);
            c.restore();
            centerTitleRect = {
                    x: x,
                    y: y,
                    width: w,
                    height: h
                }
        }
        //绘制左下对齐文本
        function drawLeftAlignText(x, y, text, erase){
            c.save();        
            c.font = valueFont;
            c.textAlign = "left";
            c.textBaseline = "bottom";
            if(erase){
                var size = c.measureText(text);
                clearTitleRect({
                    x: x,
                    y: y - valeuOffset - valueFontHeight,
                    width: size.width + valeuOffset,
                    height: valueFontHeight - valeuOffset});
                // c.fillStyle = "#faa";
                // c.fillRect(x, y - 20 - valeuOffset, size.width + valeuOffset, 20 - valeuOffset);
            }
            else{
                c.fillStyle = "#000";
                c.fillText(text, x + valeuOffset, y - valeuOffset);
            }
            c.restore();
        }
        //绘制右下对齐文本
        function drawRightAlignText(x, y, text, erase){
            c.save();        
            c.font = valueFont;
            c.textAlign = "right";
            c.textBaseline = "bottom";
            if(erase){
                var size = c.measureText(text);
                var w = size.width + valeuOffset, h = valueFontHeight - valeuOffset;
                clearTitleRect({
                    x: x - w,
                    y: y - h - valeuOffset * 2,
                    width: w,
                    height: h});
                // c.fillStyle = "#faa";
                // c.fillRect(x - w, y - h - valeuOffset * 2, w, h);
            }
            else{
                c.fillStyle = "#000";
                c.fillText(text, x - valeuOffset, y - valeuOffset);
            }
            c.restore();
        } 
        //绘制中下对齐文本
        function drawCenterAlignText(x, y, text, erase){
            c.save();        
            c.font = valueFont;
            c.textAlign = "center";
            c.textBaseline = "bottom";
            if(erase){
                var size = c.measureText(text);
                var w = size.width, h = valueFontHeight - valeuOffset;
                clearTitleRect({
                    x: x - w / 2,
                    y: y - h - valeuOffset * 2,
                    width: w,
                    height: h});
                // c.fillStyle = "#faa";
                // c.fillRect(x - w / 2,y - h - valeuOffset * 2, w, h);
            }
            else{
                c.fillStyle = "#000";
                c.fillText(text, x, y - valeuOffset);
            }
            c.restore();
        }
        //左框值
        function drawLeftTX(value){
            drawLeftAlignText(boxA_X - pinWidth, boxA_TX_Y, getLessZeroValue(leftTX, 2), true);
            leftTX = value;
            drawLeftAlignText(boxA_X - pinWidth, boxA_TX_Y, getLessZeroValue(value, 2));
        }
        function drawLeftRX(value){
            drawLeftAlignText(boxA_X - pinWidth, boxA_RX_Y, getLessZeroValue(leftRX, 2), true);
            leftRX = value;
            drawLeftAlignText(boxA_X - pinWidth, boxA_RX_Y, getLessZeroValue(value, 2));
        }
        function drawLeftT1(value){
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_T1_Y, getLessZeroValue(leftT1, 2), true);
            leftT1 = value;
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_T1_Y, getLessZeroValue(value, 2));
            drawDiff_T1_R1();
        }
        function drawLeftT2(value){
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_T2_Y, getLessZeroValue(leftT2, 2), true);
            leftT2 = value;
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_T2_Y, getLessZeroValue(value, 2));
            drawDiff_T2_R2();
        }
        function drawLeftR1(value){
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_R1_Y, getLessZeroValue(leftR1, 2), true);
            leftR1 = value;
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_R1_Y, getLessZeroValue(value, 2));
            drawDiff_R1_T1();
        }
        function drawLeftR2(value){
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_R2_Y, getLessZeroValue(leftR2, 2), true);
            leftR2 = value;
            drawLeftAlignText(boxA_X + boxWidth + boxPadding, boxA_R2_Y, getLessZeroValue(value, 2));
            drawDiff_R2_T2();
        }        
        //右框值
        function drawRightTX(value){
            drawRightAlignText(boxB_X + boxWidth + pinWidth, boxB_TX_Y, getLessZeroValue(rightTX, 2), true);
            rightTX = value;
            drawRightAlignText(boxB_X + boxWidth + pinWidth, boxB_TX_Y, getLessZeroValue(value, 2));
        }
        function drawRightT1(value){
            drawRightAlignText(boxB_X - boxPadding, boxB_T1_Y, getLessZeroValue(rightT1, 2), true);
            rightT1 = value;
            drawRightAlignText(boxB_X - boxPadding, boxB_T1_Y, getLessZeroValue(value, 2));
            drawDiff_R1_T1();
        }
        function drawRightT2(value){
            drawRightAlignText(boxB_X - boxPadding, boxB_T2_Y, getLessZeroValue(rightT2, 2), true);
            rightT2 = value;
            drawRightAlignText(boxB_X - boxPadding, boxB_T2_Y, getLessZeroValue(value, 2));
            drawDiff_R2_T2();
        }
        function drawRightRX(value){
            drawRightAlignText(boxB_X + boxWidth + pinWidth, boxB_RX_Y, getLessZeroValue(rightRX, 2), true);
            rightRX = value;
            drawRightAlignText(boxB_X + boxWidth + pinWidth, boxB_RX_Y, getLessZeroValue(value, 2));
        }
        function drawRightR1(value){
            drawRightAlignText(boxB_X - boxPadding, boxB_R1_Y, getLessZeroValue(rightR1, 2), true);
            rightR1 = value;
            drawRightAlignText(boxB_X - boxPadding, boxB_R1_Y, getLessZeroValue(value, 2));
            drawDiff_T1_R1();
        }
        function drawRightR2(value){
            drawRightAlignText(boxB_X - boxPadding, boxB_R2_Y, getLessZeroValue(rightR2, 2), true);
            rightR2 = value;
            drawRightAlignText(boxB_X - boxPadding, boxB_R2_Y, getLessZeroValue(value, 2));
            drawDiff_T2_R2();
        }
        //差值
        function drawDiff_T1_R1(){
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_R1_Y, getLessZeroValue(diff_T1_R1, 2) + "dB", true);
            diff_T1_R1 = leftT1 - rightR1;            
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_R1_Y, getLessZeroValue(diff_T1_R1, 2) + "dB");
        }
        function drawDiff_T2_R2(){
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_R2_Y, getLessZeroValue(diff_T2_R2, 2) + "dB", true);
            diff_T2_R2 = leftT2 - rightR2;
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_R2_Y, getLessZeroValue(diff_T2_R2, 2) + "dB");
        }
        function drawDiff_R1_T1(){
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_T1_Y, getLessZeroValue(diff_R1_T1, 2) + "dB", true);
            diff_R1_T1 = rightT1 - leftR1;
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_T1_Y, getLessZeroValue(diff_R1_T1, 2) + "dB");
        }
        function drawDiff_R2_T2(){
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_T2_Y, getLessZeroValue(diff_R2_T2, 2) + "dB", true);
            diff_R2_T2 = rightT2 - leftR2;
            drawCenterAlignText(boxB_X - (boxB_X - boxA_X - boxWidth) / 2, boxB_T2_Y, getLessZeroValue(diff_R2_T2, 2) + "dB");
        }
        drawStatic();
        //定义实例方法
        Object.defineProperties(this, {
            //切换到主路
            switchToPrimary: { 
                writable: false, 
                value: function(){ 
                    drawStatic();
                    drawPortLine(boxA_TX_X, boxA_TX_Y, boxA_T1_X, boxA_T1_Y, linkColor);
                    drawPortLine(lineStartX, lineY1, lineEndX, lineY1, linkColor);
                    drawPortLine(boxB_R1_X, boxB_R1_Y, boxB_RX_X, boxB_RX_Y, linkColor);
                    drawPortLine(boxA_RX_X, boxA_RX_Y, boxA_R1_X, boxA_R1_Y, linkColor);
                    drawPortLine(lineStartX, lineY3, lineEndX, lineY3, linkColor);
                    drawPortLine(boxB_T1_X, boxB_T1_Y, boxB_TX_X, boxB_TX_Y, linkColor);
                    } 
                },
            //切换到备路
            switchToSecondary:{
                writable: false,
                value: function(){
                    drawStatic();
                    drawPortLine(boxA_TX_X, boxA_TX_Y, boxA_T2_X, boxA_T2_Y, linkColor);
                    drawPortLine(lineStartX, lineY2, lineEndX, lineY2, linkColor);
                    drawPortLine(boxB_R2_X, boxB_R2_Y, boxB_RX_X, boxB_RX_Y, linkColor);
                    drawPortLine(boxA_RX_X, boxA_RX_Y, boxA_R2_X, boxA_R2_Y, linkColor);
                    drawPortLine(lineStartX, lineY4, lineEndX, lineY4, linkColor);
                    drawPortLine(boxB_T2_X, boxB_T2_Y, boxB_TX_X, boxB_TX_Y, linkColor);
                }
            },
            //设置左边标题
            setleftTitle:{
                writable:false,
                value: drawTitleLeft
            },
            //左边框顶文本
            setLeftBoxTopText:{
                writable: false,
                value: drawLeftBoxTopText
            },
            //左边框低文本
            setLeftBoxBottomText:{
                writable: false,
                value: drawLeftBoxBottomText
            },
            //设置右边标题
            setRightTitle:{
                writable: false,
                value: drawTitleRight
            },
            //右边框顶文本
            setRightBoxTopText:{
                writable: false,
                value: drawRightBoxTopText
            },
            //右边框底文本
            setRightBoxBottomText:{
                writable: false,
                value: drawRightBoxBottomText
            },
            //中间文本
            setCenterText: {
                writable: false,
                value: drawCenterText
            },
            //左侧值
            setLeftTX:{
                writable:false,
                value: drawLeftTX
            },
            setLeftT1:{
                writable:false,
                value: drawLeftT1
            },
            setLeftT2:{
                writable:false,
                value: drawLeftT2
            },
            setLeftRX:{
                writable:false,
                value: drawLeftRX
            },
            setLeftR1:{
                writable:false,
                value: drawLeftR1
            },
            setLeftR2:{
                writable:false,
                value: drawLeftR2
            },
            //右侧值
            setRightTX:{
                writable:false,
                value: drawRightTX
            },
            setRightT1:{
                writable:false,
                value: drawRightT1
            },
            setRightT2:{
                writable:false,
                value: drawRightT2
            },
            setRightRX:{
                writable:false,
                value: drawRightRX
            },
            setRightR1:{
                writable:false,
                value: drawRightR1
            },
            setRightR2:{
                writable:false,
                value: drawRightR2
            }
        })
    }
    //去除多余的零
    function getLessZeroValue(value, precision){
            if(value == 0) return "0";
            var v1 = value.toFixed(precision);
            var lastZero = -1;
            var pointPos = v1.indexOf(".");
            for(var i = v1.length - 1; i > pointPos; --i){
                if(v1.charAt(i) == "0") lastZero = i;
                else break;
            }
            if(lastZero != -1){
                if(pointPos == lastZero - 1){
                    v1 = v1.substring(0, pointPos);
                }else{
                    v1 = v1.substring(0, lastZero);
                }
            }
            return v1;
        }
    //命名空间
    if(!window.glsun) window.glsun = {};
    window.glsun.Route = Route;
}())