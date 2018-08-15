(function($){
    if(!$) return;

    //OEO卡
    function OEOCard(selector){
        //字段
        this.selector = selector;
        this.$oeo = $(selector);
        this.setting = null;
    }
    //实例方法
    OEOCard.prototype.init = function(options){
        this.setting = $.extend({},{
            oeoClick: null,
            oeoDbClick: null
        },options);
        var self = this;
        this.$oeo.click(function(){
            if(self.setting.oeoClick && typeof self.setting.oeoClick  == "function"){
                self.setting.oeoClick.call(this);
            }
        });
        this.$oeo.dblclick(function(){
            if(self.setting.oeoDbClick && typeof self.setting.oeoDbClick  == "function"){
                self.setting.oeoDbClick.call(this);
            }
        });
    }
     //闪烁
    OEOCard.prototype.ligthBlink = function (light, state) {
        if(!light || typeof light != "string") return this;
        this.$oeo.children("."+light).toggleClass(state);
        return this;
    }
    //重置电源灯等
    OEOCard.prototype.resetLight_pwr = function(){
        this.$oeo.children(".pwr").removeClass("error").removeClass("normal");
        return this;
    }
    OEOCard.prototype.setError_pwr = function(){
        this.$oeo.children(".pwr").addClass("error");
        return this;
    }
    OEOCard.prototype.setNormal_pwr = function(){
        this.$oeo.children(".pwr").addClass("normal");
        return this;
    }    
    //运行灯
    OEOCard.prototype.resetLight_run = function(){
        this.$oeo.children(".run").removeClass("error").removeClass("normal");
        return this;
    }
    OEOCard.prototype.setError_run = function(){
        this.$oeo.children(".run").addClass("error");
        return this;
    }
    OEOCard.prototype.setNormal_run = function(){
        this.$oeo.children(".run").addClass("normal");
        return this;
    }
    //模块灯
    OEOCard.prototype.resetModuleLight = function(number){
        var sel = ".module-light-" + number;
        this.$oeo.children(sel).removeClass("error").removeClass("normal");
        return this;
    }
    OEOCard.prototype.setModuleLightToError = function(number){
        var sel = ".module-light-" + number;
        this.$oeo.children(sel).addClass("error");
        return this;
    }
    OEOCard.prototype.setModuleLightToNormal = function(number){
        var sel = ".module-light-" + number;
        this.$oeo.children(sel).addClass("normal");
        return this;
    }
    //模块
    OEOCard.prototype.moduleInsert = function(number){
        var sel = ".module-" + number;
        this.$oeo.children(sel).addClass("module-exist");
        return this;
    }
    OEOCard.prototype.modulePullout = function(number){
        var sel = ".module-" + number;
        this.$oeo.children(sel).removeClass("module-exist");
        return this;
    }

    //edfa卡
    function EDFACard(selector){
        //字段
        this.selector = selector;
        this.$edfa = $(selector);
        this.setting = null;
    }
    EDFACard.prototype.init = function(options){
        this.setting = $.extend({},{
            edfaClick: null,
            edfaDbClick: null
        }, options);
        var self = this;
        this.$edfa.click(function(){
            if(self.setting.edfaClick && typeof self.setting.edfaClick  == "function"){
                self.setting.edfaClick.call(this);
            }
        });
        this.$edfa.dblclick(function(){
            if(self.setting.edfaDbClick && typeof self.setting.edfaDbClick  == "function"){
                self.setting.edfaDbClick.call(this);
            }
        });
        return this;
    }
    
    //pwr灯
    EDFACard.prototype.resetLight_pwr = function () {
        this.$edfa.children(".pwr").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_pwr = function () {
        this.$edfa.children(".pwr").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_pwr = function () {
        this.$edfa.children(".pwr").addClass("normal");
        return this;
    };
    //run灯
    EDFACard.prototype.resetLight_run = function () {
        this.$edfa.children(".run").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_run = function () {
        this.$edfa.children(".run").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_run = function () {
        this.$edfa.children(".run").addClass("normal");
        return this;
    };
    //in灯
    EDFACard.prototype.resetLight_in = function () {
        this.$edfa.children(".in").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_in = function () {
        this.$edfa.children(".in").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_in = function () {
        this.$edfa.children(".in").addClass("normal");
        return this;
    };
    //out灯
    EDFACard.prototype.resetLight_out = function () {
        this.$edfa.children(".out").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_out = function () {
        this.$edfa.children(".out").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_out = function () {
        this.$edfa.children(".out").addClass("normal");
        return this;
    };
    //mt灯
    EDFACard.prototype.resetLight_mt = function () {
        this.$edfa.children(".mt").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_mt = function () {
        this.$edfa.children(".mt").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_mt = function () {
        this.$edfa.children(".mt").addClass("normal");
        return this;
    };
    //pt灯
    EDFACard.prototype.resetLight_pt = function () {
        this.$edfa.children(".pt").removeClass("error").removeClass("normal");
        return this;
    };
    EDFACard.prototype.setError_pt = function () {
        this.$edfa.children(".pt").addClass("error");
        return this;
    };
    EDFACard.prototype.setNormal_pt = function () {
        this.$edfa.children(".pt").addClass("normal");
        return this;
    };
    //闪烁
    EDFACard.prototype.ligthBlink = function (light, state) {
        if(!light || typeof light != "string") return this;
        this.$edfa.children("."+light).toggleClass(state);
        return this;
    }

    //OLP卡
    function OLPCard(selector){
        //字段
        this.selector = selector;
        this.$olp = $(selector);
        this.setting = null;
    }
    OLPCard.prototype.init = function(options){
        this.setting = $.extend({},{
            olpClick: null,
            olpDbClick: null
        }, options);
        var self = this;
        this.$olp.click(function(){
            if(self.setting.olpClick && typeof self.setting.olpClick  == "function"){
                self.setting.olpClick.call(this);
            }
        });
        this.$olp.dblclick(function(){
            if(self.setting.olpDbClick && typeof self.setting.olpDbClick  == "function"){
                self.setting.olpDbClick.call(this);
            }
        });
        return this;
    }
    OLPCard.prototype.resetLight = function(light){
        this.$olp.children("."+light).removeClass("error").removeClass("normal");
        return this;
    };
    OLPCard.prototype.setLightNormal = function(light){
        this.$olp.children("."+light).addClass("normal");
        return this;
    };
    OLPCard.prototype.setLightError = function(light){
        this.$olp.children("."+light).addClass("error");
        return this;
    };
    //闪烁
    OLPCard.prototype.ligthBlink = function (light, state) {
        this.$olp.children("."+light).toggleClass(state);
        return this;
    };
    //pwr灯
    OLPCard.prototype.resetLight_pwr = function () {
        return this.resetLight("pwr");
    };
    OLPCard.prototype.setNormal_pwr = function () {
        return this.setLightNormal("pwr");
    };
    OLPCard.prototype.setError_pwr = function () {
        return this.setLightError("pwr");
    };
    //run灯
    OLPCard.prototype.resetLight_run = function () {
        return this.resetLight("run");
    };
    OLPCard.prototype.setNormal_run = function () {
        return this.setLightNormal("run");
    };
    OLPCard.prototype.setError_run = function () {
        return this.setLightError("run");
    };
    //alm灯
    OLPCard.prototype.resetLight_alm = function () {
        return this.resetLight("alm");
    };
    OLPCard.prototype.setNormal_alm = function () {
        return this.setLightNormal("alm");
    };
    OLPCard.prototype.setError_alm = function () {
        return this.setLightError("alm");
    };
    //auto灯
    OLPCard.prototype.resetLight_auto = function () {
        return this.resetLight("auto");
    };
    OLPCard.prototype.setNormal_auto = function () {
        return this.setLightNormal("auto");
    };
    OLPCard.prototype.setError_auto = function () {
        return this.setLightError("auto");
    };
    //r1灯
    OLPCard.prototype.resetLight_r1 = function () {
        return this.resetLight("r1");
    };
    OLPCard.prototype.setNormal_r1 = function () {
        return this.setLightNormal("r1");
    };
    OLPCard.prototype.setError_r1 = function () {
        return this.setLightError("r1");
    };
    //r2灯
    OLPCard.prototype.resetLight_r2 = function () {
        return this.resetLight("r2");
    };
    OLPCard.prototype.setNormal_r2 = function () {
        return this.setLightNormal("r2");
    };
    OLPCard.prototype.setError_r2 = function () {
        return this.setLightError("r2");
    };
    //prl灯
    OLPCard.prototype.resetLight_prl = function () {
        return this.resetLight("prl");
    };
    OLPCard.prototype.setNormal_prl = function () {
        return this.setLightNormal("prl");
    };
    OLPCard.prototype.setError_prl = function () {
        return this.setLightError("prl");
    };
    //tx灯
    OLPCard.prototype.resetLight_tx = function () {
        return this.resetLight("tx");
    };
    OLPCard.prototype.setNormal_tx = function () {
        return this.setLightNormal("tx");
    };
    OLPCard.prototype.setError_tx = function () {
        return this.setLightError("tx");
    };
    //ls灯
    OLPCard.prototype.resetLight_ls = function () {
        return this.resetLight("ls");
    };
    OLPCard.prototype.setNormal_ls = function () {
        return this.setLightNormal("ls");
    };
    OLPCard.prototype.setError_ls = function () {
        return this.setLightError("ls");
    };

    /*风扇*/
    function FAN(selector){
        //字段
        this.selector = selector;
        this.$fan = $(selector);
        this.setting = null;
    }
    FAN.prototype.init = function(options){
        this.setting = $.extend({},{
            fanClick: null
        },options);
        var self = this;
        this.$fan.click(function(){
            if(self.setting.fanClick && typeof self.setting.fanClick == "function"){
                self.setting.fanClick.call(this);
            }
        });
    }
    FAN.prototype.resetLight = function(light){
        this.$fan.children("."+light).removeClass("error").removeClass("normal");
        return this;
    };
    FAN.prototype.setLightNormal = function(light){
        this.$fan.children("."+light).addClass("normal");
        return this;
    };
    FAN.prototype.setLightError = function(light){
        this.$fan.children("."+light).addClass("error");
        return this;
    };

    //电源灯
    FAN.prototype.resetLight_pwr = function(){
        return this.resetLight("pwr");
    }
    FAN.prototype.setLightNormal_pwr = function(){
        return this.setLightNormal("pwr");
    }
    FAN.prototype.setLightError_pwr = function(){
        return this.setLightError("pwr");
    }
    //状态灯
    FAN.prototype.resetLight_state = function(){
        return this.resetLight("state");
    }
    FAN.prototype.setLightNormal_state = function(){
        return this.setLightNormal("state");
    }
    FAN.prototype.setLightError_state = function(){
        return this.setLightError("state");
    }
    FAN.prototype.lightBlink = function (light, state) {
        this.$fan.children("." + light).toggleClass(state);
        return this;
    }
    //电口主控
    function ElectricMCU(selector){
        //字段
        this.selector = selector;
        this.$mcu = $(selector);
        this.setting = null;
    }
    ElectricMCU.prototype.init = function (options) {
        this.setting = $.extend({},{
            mcuClick: null
        },options);
        var self = this;
        this.$mcu.click(function(){
            if(self.setting.mcuClick && typeof self.setting.mcuClick  == "function"){
                self.setting.mcuClick.call(this);
            }
        });
    };
    //重置电源灯等
    ElectricMCU.prototype.resetPowerLight1 = function(){
        this.$mcu.children(".pwr1").removeClass("error").removeClass("normal");
        return this;
    }
    ElectricMCU.prototype.setPowerLight1ToError = function(){
        this.$mcu.children(".pwr1").addClass("error");
        return this;
    }
    ElectricMCU.prototype.setPowerLight1ToNormal = function(){
        this.$mcu.children(".pwr1").addClass("normal");
        return this;
    }   
    //重置电源灯等
    ElectricMCU.prototype.resetPowerLight2 = function(){
        this.$mcu.children(".pwr2").removeClass("error").removeClass("normal");
        return this;
    }
    ElectricMCU.prototype.setPowerLight2ToError = function(){
        this.$mcu.children(".pwr2").addClass("error");
        return this;
    }
    ElectricMCU.prototype.setPowerLight2ToNormal = function(){
        this.$mcu.children(".pwr2").addClass("normal");
        return this;
    }   
    //运行灯
    ElectricMCU.prototype.resetRunningLight = function(){
        this.$mcu.children(".run").removeClass("error").removeClass("normal");
        return this;
    }
    ElectricMCU.prototype.setRunningLightToError = function(){
        this.$mcu.children(".run").addClass("error");
        return this;
    }
    ElectricMCU.prototype.setRunningLightToNormal = function(){
        this.$mcu.children(".run").addClass("normal");
        return this;
    }
    ElectricMCU.prototype.runningLightBlinking = function(){
        this.$mcu.children(".run").toggleClass("normal");
        return this;
    }

    function Device(selector){
        var imgPath = "../../image/device/";
        if (!selector || selector == "") selector = "table tbody";
        this.$tableBody = $(selector);
        //静态内容
        this.imgPath = imgPath;
        this.emptyMCU = '<td rowspan="2"><div id="mcu" class="module"><img src="' + imgPath + 'mcu-frame.jpg"></div></td>';
        this.oeoContent = '<div class="light-circle pwr"></div><div class="light-circle run"></div><img class="sfp-module module-1 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-2 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-3 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-4 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-5 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-6 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-7 module-exist"src="' + imgPath + 'SFP-module.jpg"><img class="sfp-module module-8 module-exist"src="' + imgPath + 'SFP-module.jpg"><div class="light-rectangle module-light-1"></div><div class="light-rectangle module-light-2"></div><div class="light-rectangle module-light-3"></div><div class="light-rectangle module-light-4"></div><div class="light-rectangle module-light-5"></div><div class="light-rectangle module-light-6"></div><div class="light-rectangle module-light-7"></div><div class="light-rectangle module-light-8"></div>';
        this.edfaContent = '<div class="light-circle pwr"></div><div class="light-circle run"></div><div class="light-circle in"></div><div class="light-circle out"></div><div class="light-circle mt"></div><div class="light-circle pt"></div>';
        this.olpContent = '<div class="light-circle pwr"></div><div class="light-circle run"></div><div class="light-circle alm"></div><div class="light-circle auto"></div><div class="light-circle r1"></div><div class="light-circle r2"></div><div class="light-circle prl"></div><div class="light-circle tx"></div><div class="light-circle ls"></div>';
        this.mcuContent = '<div class="light-circle pwr1"></div><div class="light-circle pwr2"></div><div class="light-circle run"></div>';
        
        //动态字段
        this.u = "";
        this.mcu_type = "";
        this.mcu = null;
        this.fan = null;
        this.cards = {};
        this.setting = null;
        var self = this;
        this.moduleClick = function(){
            var $this = $(this);
            //console.log(this.id + " click");
            if($this.hasClass("highLight")){

            }else{
                var modules = $(".module");
                modules.each(function(){
                    removeHighLight($(this));
                });
                addhighLight($(this));
            }
            if(self.setting.click && typeof self.setting.click  == "function"){
                self.setting.click.call(this);
            }
        };
        this.moduleDbClick = function(){
            if(self.setting.dblclick && typeof self.setting.dblclick  == "function"){
                self.setting.dblclick.call(this);
            }
        };
    }
    //空卡HTML内容
    Device.prototype.getemptyCard = function(slot) {  
        return '<td><div id="card-slot-'+slot+'" class="module"><img src="' + this.imgPath + 'card-frame.jpg"></div></td>';
    }
    //初始化
    Device.prototype.init = function(mcu_type, u, options){
        this.setting = $.extend({},{
            click: function(){},
            dblclick: function(){},
            afterInit: function(){}
        },options);
        this.mcu_type = mcu_type;
        this.u = u;
        var slot = 1;
        var fanClass = "fan-"+u+"u";
        this.$tableBody.empty();
        for(var i = 1; i <= u; ++i){
            strU = '<tr>';
            strU += this.emptyMCU;
            strU += this.getemptyCard(slot++);
            strU += this.getemptyCard(slot++);
            if(i == 1){
                var img = "";
                if(u == 1){
                    img = "1U-FAN.jpg";
                }
                else if(u == 2){
                    img = "2U-FAN.jpg";
                }
                else if(u == 4){
                    img = "4U-FAN.jpg";
                }
                var fan = '<td rowspan="' + u*2 + '"><div id="fan" class="module '+fanClass+'"><img src="' + this.imgPath + img + '"><div class="light-circle pwr"></div><div class="light-circle state"></div></div></td>'
                strU += fan;
            }
            strU += '</tr>';
            this.$tableBody.append(strU);
            strU = '<tr>'
            strU += this.getemptyCard(slot++) + this.getemptyCard(slot++);
            strU += '</tr>';
            this.$tableBody.append(strU);
        }
        this.fan = new FAN("." + fanClass);
        this.fan.init({
            fanClick: this.moduleClick
        });
        if(mcu_type != ""){
            this.insertMCU(mcu_type);
        }
        if(this.setting.afterInit && typeof this.setting.afterInit == "function"){
            this.setting.afterInit.call();
        }
        return this;
    };
    //插入主控
    Device.prototype.insertMCU = function(type) {
        var mcu = $("#mcu")
        if(type == "optics"){
            mcu.addClass("mc-optics");
            mcu.children("img:first").attr("src", this.imgPath + "mcu-" + type + ".jpg");
        }
        else if(type == "electronic"){
            mcu.addClass("mc-electronic");
            mcu.children("img:first").attr("src", this.imgPath + "mcu-" + type + ".jpg");
        }
        mcu.append(this.mcuContent);
        //创建一个MCU控件
        this.mcu = new ElectricMCU("#mcu");
        this.mcu.init({
            mcuClick: this.moduleClick
        });
        return this;
    };
    //拨出主控
    Device.prototype.removeMCU = function () {
        var mcu = $("#mcu");
        mcu.unbind("click");
        mcu.children("div").remove();
        mcu.children("img:first").attr("src", this.imgPath + "mcu-frame.jpg");
        this.mcu = null;
    };
    //插入板卡
    Device.prototype.insertCard = function (slot, type) {
        var selector = "#card-slot-" + slot;
        var card = $(selector);
        card.addClass(type.toLowerCase());
        card.children("img").attr("src", this.imgPath + type.toUpperCase()+".jpg");
        var ctrl = null;
        type = type.toLowerCase();
        if(type == "oeo"){
            card.append(this.oeoContent);
            ctrl = new OEOCard(selector);
            ctrl.init({
                oeoClick: this.moduleClick,
                oeoDbClick: this.moduleDbClick
            });
        }
        else if(type == "olp"){
            card.append(this.olpContent);
            ctrl = new OLPCard(selector);
            ctrl.init({
                olpClick: this.moduleClick,
                olpDbClick: this.moduleDbClick
            });
        }
        else if(type == "edfa"){
            card.append(this.edfaContent);
            ctrl = new EDFACard(selector);
            ctrl.init({
                edfaClick: this.moduleClick,
                edfaDbClick: this.moduleDbClick
            });
        }
        this.cards[selector.substring(1)] = {
            type: type,
            card: ctrl
        };
        return this;
    }
    //拨出板卡
    Device.prototype.removeCard = function(slot){
        var selector = "#card-slot-" + slot;
        var card = $(selector);
        var divs = card.children("div");
        var imgs = card.children(":not(img:first)");
        card.children("img:first").attr("src", this.imgPath + "card-frame.jpg");
        card.unbind("click");
        divs.remove();
        imgs.remove();
        this.cards[selector.substring(1)] = null;
        return this;
    }
    //获取卡槽上的板卡
    Device.prototype.getCard = function(slot){
        return this.cards["card-slot-" + slot];
    }
    //高亮
    function addhighLight($div) {
        $div.addClass("highLight");
        $div.children("img:first").css("opacity", 0.9);
    }
    //不高亮
    function removeHighLight($div) {
        $div.removeClass("highLight");
        $div.children("img:first").css("opacity", 1);
    }
    //命名空间
    if(!window.glsun) window.glsun = {};
    window.glsun.OEOCard = OEOCard;
    window.glsun.EDFACard = EDFACard;
    window.glsun.OLPCard = OLPCard;
    window.glsun.FAN = FAN;
    window.glsun.ElectricMCU = ElectricMCU;
    window.glsun.Device = Device;
}(jQuery));