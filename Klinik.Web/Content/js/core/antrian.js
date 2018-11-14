function onCallback(data){
    console.log(data);
}

var queueAntrian = new Array();
var counterAntrian = 0;
function panggilPasien(num, noPoli){
    
    $.ajax({
        type: 'GET',
        url: antrianURL + "/site/panggil/"+num,
        dataType: 'jsonp',
        success: function (data) {
        }
    });
    
    return;
    queueAntrian = new Array();
    queueAntrian.push(getBuzz("pasien_nomor"));
    //decode angka
    decodeAngka(num);
    queueAntrian.push(getBuzz("masuk_ke_nomor"));
    queueAntrian.push(getBuzz(noPoli));
    counterAntrian = 0;
    playRecursive();
}

function playRecursive(){
    var suara = queueAntrian[counterAntrian];
    counterAntrian++;
    if(suara != null){
        suara.bind("ended", function(e) {
            playRecursive();
        });
        suara.play();
    }
}

function decodeAngka(num){
    if(num <= 11 || num == 100){
        queueAntrian.push(getBuzz(num));
    }else if(num >= 12 && num <= 19){
        var angkaBelakang = num - 10;
        queueAntrian.push(getBuzz(angkaBelakang));
        queueAntrian.push(getBuzz("belas"));
    }else if(num >= 20 && num <= 99){
        var puluhan = Math.floor(num / 10);
        queueAntrian.push(getBuzz(puluhan));
        queueAntrian.push(getBuzz("puluh"));
        if(num % 10 != 0){
            queueAntrian.push(getBuzz(num % 10));
        }
    }else if(num > 99){
        //ratusan
        var ratusan = Math.floor(num / 100);
        if(ratusan == 1){
            queueAntrian.push(getBuzz("100"));
        }else{
            queueAntrian.push(getBuzz(ratusan));
            queueAntrian.push(getBuzz("ratus"));
        }
        if(num % 100 != 0){
            decodeAngka(num % 100);
        }
    }
}

function getBuzz(fileName){
    return new buzz.sound(baseURL+"/static/sound/"+fileName+".wav");
}