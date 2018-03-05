var budgetItems = [
        { id: 1, pId: 0, name: "一、本期收入", open: true    },
        { id: 11, pId: 1, name: "1.项目净往来" },
        { id: 12, pId: 1, name: "2.租金净收入" },
        { id: 13, pId: 1, name: "3.融资收入" },
        { id: 15, pId: 1, name: "4.股权收入" },
        { id: 16, pId: 1, name: "5.其它收入" },
             { id: 2, pId: 0, name: "二、本期支出", open: true },
        { id: 21, pId: 2, name: "1.日常支出" },
        { id: 22, pId: 2, name: "2.财务费用" },
        { id: 23, pId: 2, name: "3.其他支出" },
        { id: 24, pId: 2, name: "4.融资还本" },
        { id: 25, pId: 2, name: "5.各系统投资" },
        { id: 26, pId: 2, name: "6.新投项目" }

];

var wandaCompanies = [
        { id: 1, pId: 0, name: "集团总部", open: true, nocheck: true }, 
        { id: 2, pId: 0, name: "商业地产", open: true, nocheck: true },
            { id: 21, pId: 2, name: "成都金牛" },
            { id: 22, pId: 2, name: "大连东港" },
            { id: 23, pId: 2, name: "大连高新" },
            { id: 24, pId: 2, name: "蚌埠项目" },
            { id: 25, pId: 2, name: "赤峰项目" },
            { id: 201, pId: 2, name: "上海松江" },
            { id: 202, pId: 2, name: "上海宝山" },
            { id: 203, pId: 2, name: "上海江桥" },
            { id: 204, pId: 2, name: "上海周浦" },
            { id: 205, pId: 2, name: "上海五角场" },
            { id: 206, pId: 2, name: "丹东项目" },
            { id: 207, pId: 2, name: "潍坊项目" },
            { id: 208, pId: 2, name: "包头青山" },
            { id: 209, pId: 2, name: "北京石景山" },
            { id: 210, pId: 2, name: "北京CBD" },
            { id: 211, pId: 2, name: "长春宽城" },
            { id: 212, pId: 2, name: "长春红旗街" },
            { id: 213, pId: 2, name: "成都金牛" },
            { id: 214, pId: 2, name: "成都锦华城" },
            { id: 215, pId: 2, name: "哈尔滨哈西" },
            { id: 216, pId: 2, name: "哈尔滨香坊" },
            { id: 217, pId: 2, name: "呼和浩特" },
            { id: 218, pId: 2, name: "淮安项目" },
            { id: 219, pId: 2, name: "济南魏家庄" },
            { id: 220, pId: 2, name: "石家庄裕华" },
            { id: 221, pId: 2, name: "苏州太仓" },
            { id: 222, pId: 2, name: "苏州平江" },
            { id: 223, pId: 2, name: "唐山项目" },
            { id: 224, pId: 2, name: "天津万达中心" },
            { id: 225, pId: 2, name: "天津河东" },
            { id: 226, pId: 2, name: "江阴项目" },
            { id: 227, pId: 2, name: "宜兴项目" },
            { id: 228, pId: 2, name: "无锡滨湖" },
            { id: 229, pId: 2, name: "武汉东湖" },
            { id: 230, pId: 2, name: "武汉积玉桥" },
            { id: 231, pId: 2, name: "武汉菱角湖" },
            { id: 232, pId: 2, name: "武汉经开" },
            { id: 233, pId: 2, name: "武汉汉街" },
            { id: 234, pId: 2, name: "宜昌项目" },
            { id: 235, pId: 2, name: "重庆南坪" },
            { id: 236, pId: 2, name: "重庆万州项目" },
            { id: 237, pId: 2, name: "绍兴柯桥" },
            { id: 238, pId: 2, name: "合肥天鹅湖" },
            { id: 239, pId: 2, name: "合肥包河" },
           
                                     

        { id: 3, pId: 0, name: "文化集团总部" },
            { id: 31, pId: 3, name: "成都金牛" },
            { id: 32, pId: 3, name: "大连东港" }
];
 


/*
上海松江
上海宝山
上海江桥
上海周浦
上海五角场
丹东项目
潍坊项目
包头青山
北京石景山
北京CBD
长春宽城
长春红旗街
成都金牛
成都锦华城
哈尔滨哈西
哈尔滨香坊
呼和浩特
淮安项目
济南魏家庄
石家庄裕华
苏州太仓
苏州平江
唐山项目
天津万达中心
天津河东
江阴项目
宜兴项目
无锡滨湖
武汉东湖
武汉积玉桥
武汉菱角湖
武汉经开
武汉汉街
宜昌项目
重庆南坪
重庆万州项目
绍兴柯桥
合肥天鹅湖
合肥包河
襄阳项目
福州仓山项目
福州金融街
银川金凤
广州白云
长沙开福
郑州中原
郑州二七
常州新北
泉州浦西
晋江项目
南昌红谷滩
宁德项目
青岛李沧
青岛CBD
温州龙湾
漳州碧湖
芜湖镜湖
莆田项目
绵阳涪城
厦门集美
厦门湖里
三亚酒店
广州增城项目
洛阳项目
东莞东城
芜湖二期
昆明CBD
佛山金融区
银川西夏
银川酒店
满洲里项目
兰州城关
金华项目
马鞍山项目
南宁项目
烟台芝罘项目
常州武进
绵阳CBD
济宁太白路项目
龙岩项目
福州福清
荆州项目
营口项目
齐齐哈尔项目
长春重庆路项目
青岛台东项目
天津和平金街项目
*/