(function($){
    $.validator.addMethod("password",function(value,element,params){  
        if(params[0] == false){
            return true;
        }
        var pattern = /^(\w){0,20}$/
        return pattern.exec(value);
    }, "只能输入字母、数字、下划线");
    $.validator.addMethod("integer", function (value, element, params) {
        if (params[0] == false)
            return true;
        var pattern = /^\-?\d+$/;
        return pattern.exec(value);
    }, "请输入整数");
}(jQuery))