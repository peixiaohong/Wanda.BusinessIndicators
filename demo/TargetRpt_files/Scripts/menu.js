// JavaScript Document
	   // 判断浏览器的变量
	   IE4 = (navigator.appName == "Microsoft Internet Explorer") ? true : false;

	  // 初始化菜单
	  function initIt(){
		if (!IE4) return;
		divColl = document.getElementsByTagName("DIV");
		for (i=0; i<divColl.length; i++) {
		   whichele = divColl(i);
		   if (whichele.className == "child") whichele.style.display = "none";
		}
		var myLinks = document.links;
	   
		// 去掉超链接虚边框
		for( j=0;  j<myLinks.length; j++){
		  myLinks[j].onfocus = function(){this.blur();}
		}
	 }
	 // 展开菜单的方法
	 function expandIt(ele) {
	   if (!IE4) return;
	   var name = ele + "Child";
	   whichele = document.all[name];
		
	   if (whichele.style.display == "none") {
		 whichele.style.display = "block";
	   }else {
		 whichele.style.display = "none";
	   }
	 }
	 // 改变链接图片
	 function changeImg(img){
	   img.src = (img.src.indexOf("images/2.jpg") != -1)?"images/1.jpg":"images/2.jpg";
	 }
	 // 鼠标悬停的时候改变颜色
	 function mouseOn(obj){
	   obj.bgColor = "#ffffff";
	 }
	 // 鼠标离开的时候改变颜色
	 function mouseOut(obj){
	   obj.bgColor = "#ffffff";
	 }
	 onload = initIt;