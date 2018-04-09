// JavaScript Document
$(function(){
	$('.tab_1,.tab_2,.tab_4,.tab_5,.tab_9,.list_tab').each(function(){
	//$(this).find('tr:even').css("background","#f6f6f6");
	//$(this).find('tr:odd').css("background","#fff");
	});
	});
	
function opTab(){
	 var menu = document.getElementById("side");
	 var arrdiv = document.getElementById("arrow");
	 var arrtxt = document.getElementById("arrtxt");
	 if((menu.style.display == "block")|| (menu.style.display == "")){
		  menu.style.display = "none"
		  if(navigator.userAgent.indexOf("Firefox")>0){
				 arrdiv.style.left=0+'px';
				 document.getElementById("main").style.left=0+'px';
		   }else if(navigator.userAgent.indexOf("MSIE")>0){
				 arrdiv.style.pixelLeft = 0; // pixelLeft元素左边界偏移量(left值呗)
				document.getElementById("main").style.pixelLeft = 0;
		   }
		   arrtxt.innerText = "显示菜单";         

	 } else {
			menu.style.display = "block";
			if(navigator.userAgent.indexOf("Firefox")>0){
				  arrdiv.style.left=183+'px';
				  document.getElementById("main").style.left=210+'px';
			}else if(navigator.userAgent.indexOf("MSIE")>0){
				  arrdiv.style.pixelLeft = 183; 
				  document.getElementById("main").style.pixelLeft = 210;
			 }
			  arrtxt.innerText = "隐藏菜单";        

	 }
	 event.cancelBubble = true;
   }