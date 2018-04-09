var status = 1; 
 
function switchSysBar(){
	var switchPoint=document.getElementById("switchPoint");
	var frmTitle=document.getElementById("frmTitle");
     if (1 == window.status){
		  window.status = 0;
		  //alert(switchPoint);

		  switchPoint.style.backgroundImage = 'url(../images2/shrink.png)';
          frmTitle.style.display="none"
     }
     else{
		  window.status = 1;
		  switchPoint.style.backgroundImage = 'url(../images2/spread.png)';
          frmTitle.style.display=""
     }
}
 
 
