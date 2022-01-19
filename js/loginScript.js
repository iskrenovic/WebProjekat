import { KlasaIscrtavanja } from "./klasaIscrtavanja.js";

var loginPage = true;

document.body.onload
{    
    var main = document.querySelector("#main"); // # - id . - klasa
    main.className = "mainLogin";  
    drawItems(["Username", "Password"], ["text", "password"], "Log in to your account:", ["Log in", "Register as owner"]);
}


function drawItems(fieldsToDraw, fieldsType, title, buttonText){
    main.innerHTML = "";

    let forma = document.createElement("div");
    forma.className = "formica";
    main.appendChild(forma);

    let tit = document.createElement("h1");
    tit.innerHTML = title;
    forma.appendChild(tit);

    let inputDiv, label, inp;
    fieldsToDraw.forEach((fieldText, index) => {
        inputDiv = document.createElement("div");
        inputDiv.className = "inputClass";
        forma.appendChild(inputDiv);        
    
        label = document.createElement("label");
        label.innerHTML = fieldText;
        label.className = "inputLabelClass"
        inputDiv.appendChild(label);

        inp = document.createElement("input");
        inp.type = fieldsType[index];
        inp.id = `${fieldText}Inp`
        inp.className = "inputFieldClass"
        inputDiv.appendChild(inp);
    });

    let button = document.createElement("button");
    button.innerHTML = buttonText[0];
    forma.appendChild(button);
    button.addEventListener("click",function(){continueClick(buttonText[0] == "Log in")});

    button = document.createElement("button");
    button.innerHTML = buttonText[1];
    forma.appendChild(button);
    button.addEventListener("click",registerClick);

    
}

function continueClick(isLogin){
    var username = document.querySelector("#UsernameInp").value;
    var password = document.querySelector("#PasswordInp").value;
    if(isLogin){       
        fetch(`https://localhost:5001/User/LoginUser/${username}/${password}`, {
            headers: {
                "Content-Type": "application/json"
            },
            method:'GET'})
        .then(data =>{ 
            if(data.ok){       
                data.json().then(podaci=>{                        
                        let crtaj = new KlasaIscrtavanja(podaci);
                        crtaj.draw(main);
                });        
            }
            else{
                data.json().then(podaci=>{
                    alert(podaci);
                });
            }        
        }).catch(err =>{
            alert(err);
        });
    }
    else{//Register
        var name = document.querySelector("#NameInp").value;
        var surname = document.querySelector("#SurnameInp").value;

        let user = {"username": username, "password": password, "ime" :name, "prezime" :surname};
        fetch(`https://localhost:5001/User/AddOwner`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'POST',
        body: JSON.stringify(user)}).then(data=>{
            if(data.ok){
                data.json().then(podaci=>{
                    let crtaj = new KlasaIscrtavanja(podaci);
                    crtaj.draw(main);
                });
            }   
            else{             
                                   
                
            }
        }).catch(err=>{
            console.log(err);
        })
    }
}


function registerClick(){
    if(loginPage){
        drawItems(["Username", "Password", "Name", "Surname"], ["text", "password", "text", "text"], "Register as owner:", ["Register", "Login"]);
    }
    else{
        drawItems(["Username", "Password"], ["text", "password"], "Log in to your account:", ["Log in", "Register as owner"]);
    }
    loginPage = !loginPage;
}
