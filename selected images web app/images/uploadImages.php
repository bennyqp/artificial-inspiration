<?php

if(isset($_POST['imagesList'])){
        file_put_contents($_POST['sessionCode'] , $_POST['imagesList']);      
        echo "uploaded.";
    }else{
        echo "invalid file uploaded.";
    }  

?> 