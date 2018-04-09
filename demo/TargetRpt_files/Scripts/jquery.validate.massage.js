jQuery.extend(jQuery.validator.messages, {
    required: "<b style='color:red'>*</b>",
    remote: "请修正该字段",
    email: "<span style='color:red'>邮件格式不正确</span>",
    url: "请输入合法的网址",
    date: "请输入合法的日期",
    dateISO: "请输入合法的日期 (ISO).",
    number: "<span style='color:red'>请输入合法的数字</span>",
    digits: "<span style='color:red'>只能输入整数</span>",
    creditcard: "<span style='color:red'>请输入合法的信用卡号</span>",
    equalTo: "<span style='color:red'>请再次输入相同的值</span>",
    accept: "<span style='color:red'>请输入拥有合法后缀名的字符串</span>",
    maxlength: jQuery.validator.format("<span style='color:red'>请输入一个 长度最多是 {0} 的字符串</span>"),
    minlength: jQuery.validator.format("<span style='color:red'>请输入一个 长度最少是 {0} 的字符串</span>"),
    rangelength: jQuery.validator.format("<span style='color:red'>请输入 一个长度介于 {0} 和 {1} 之间的字符串</span>"),
    range: jQuery.validator.format("<span style='color:red'>请输入一个介于 {0} 和 {1} 之间的值</span>"),
    max: jQuery.validator.format("<span style='color:red'>请输入一个最大为{0} 的值</span>"),
    min: jQuery.validator.format("<span style='color:red'>请输入一个最小为{0} 的值</span>")
});