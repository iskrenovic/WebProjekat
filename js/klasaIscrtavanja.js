export class KlasaIscrtavanja{    
    constructor(us){
        this.user = us;
        this.selectedHotel = null;
        this.selectedRoom = null;
        this.selectedClient = null;
        this.listsDiv = null;               
        this.calendarDiv = null;
        this.inputDiv = null;  
        this.inputFormDiv = null;
        this.bookRoomDiv = null;
        this.bookroomFormDiv = null;    
    }


    loadCalendar(hotelName){
        console.log("LOADING CALENDAR");
         fetch(`https://localhost:5001/Booking/GetMonth/${hotelName}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'GET'})
        .then(data =>{ 
            console.log(data);
            if(data.ok){       
                data.json().then(podaci=>{
                    this.drawCalendar(podaci);                        
                });
            }
            else{
                data.json().then(podaci=>{
                    alert(podaci.message);
                });
            }        
        }).catch(err =>{
            alert(err);
        });
    }   

    loadHotels(){
        this.selectedHotel = null;
        var userId = this.user.id;
        fetch(`https://localhost:5001/Employee/GetWorkplace/${userId}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'GET'})
        .then(data =>{ 
            if(data.ok){       
                data.json().then(podaci=>{
                        console.log(podaci);                        
                        this.drawList(document.querySelector("#hotelsDiv"),"Hotels:","hotel", podaci);
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

    loadRooms(hotelName){
        this.selectedRoom = null;   
        fetch(`https://localhost:5001/Room/GetRooms/${hotelName}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'GET'})
        .then(data =>{ 
            if(data.ok){       
                data.json().then(podaci=>{
                        this.drawList(document.querySelector("#roomsDiv"),"Rooms:","room",podaci);
                        this.loadCalendar(hotelName);
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

    loadClients(hotelName){
        this.selectedClient = null;
        fetch(`https://localhost:5001/Client/GetClientsForHotel/${hotelName}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'GET'})
        .then(data =>{ 
            if(data.ok){       
                data.json().then(podaci=>{
                        this.drawList(document.querySelector("#clientsDiv"),"Clients:", "clients", podaci);
                });
            }
            else{
                data.json().then(podaci=>{
                    alert(podaci.message);
                });
            }        
        }).catch(err =>{
            alert(err);
        });
    }

    draw(host){
        host.innerHTML = "";
        host.className = "mainHome";
        let sidePannel = document.createElement("div");
        sidePannel.className = "sidePannelClass";
        host.appendChild(sidePannel);
        let mainPannel = document.createElement("div");
        mainPannel.className = "mainPannelClass";
        host.appendChild(mainPannel);
        this.listsDiv = document.createElement("div");        
        this.listsDiv.className ="divList";
        mainPannel.appendChild(this.listsDiv);

        var listDivsToCreate;
        var titles;
        
        switch(this.user.privilege){
            case 0: case 1: 
                listDivsToCreate = ["hotelsDiv","roomsDiv","clientsDiv"];
                titles = ["Hotels:", "Rooms:", "Clients"];
                
             break;
            case 2:
                listDivsToCreate = ["roomsDiv","clientsDiv"];
                titles = ["Rooms:", "Clients"]; break;
        }

        listDivsToCreate.forEach((list,index)=>{
            let div = document.createElement("div");
            div.className ="sublistDiv";
            div.id = list;
            this.listsDiv.appendChild(div);

            let title = document.createElement("h2");
            title.innerHTML = titles[index];
            div.appendChild(title);
        })        

        this.calendarDiv = document.createElement("div");
        this.calendarDiv.className = "calendar"
        mainPannel.appendChild(this.calendarDiv);

        this.bookRoomDiv = document.createElement("div");
        this.bookRoomDiv.className = "sideFunctionList"
        sidePannel.appendChild(this.bookRoomDiv);

        this.bookroomFormDiv = document.createElement("div");
        this.bookroomFormDiv.className = "formDiv"
        sidePannel.appendChild(this.bookroomFormDiv);


        this.drawInput(this.bookRoomDiv,["NEW RESERVATION"],["CANCEL A RESERVATION"]);

        this.inputDiv = document.createElement("div");
        this.inputDiv.className = "sideFunctionList"
        sidePannel.appendChild(this.inputDiv);
        
        

        this.inputFormDiv = document.createElement("div");
        this.inputFormDiv.className = "formDiv"
        sidePannel.appendChild(this.inputFormDiv);

        var listOfCreateCommands;
        var listOfDeleteCommands;        
        switch(this.user.privilege){
            case 0:
                listOfCreateCommands = ["Add a user","Add a hotel", "Add a room"];
                listOfDeleteCommands = ["Delete a user","Delete a hotel", "Delete a room"];
                this.drawAdmin(); 
            break;
            case 1:
                listOfCreateCommands = ["Add a room"];
                listOfDeleteCommands = ["Delete a room"];
                this.drawAdmin();
            break;            
            case 2:
                this.drawUser();                
            break;
        }



        this.drawInput(this.inputDiv,listOfCreateCommands, listOfDeleteCommands);        
    }

    drawAdmin(){        
        this.loadHotels();
    }    

    drawUser(){
        let hotelName = this.user.hotels[0].hotel.name;        
        this.loadRooms(hotelName);
        this.loadClients(hotelName);                     
    }

    drawCalendar(reservations){

        this.calendarDiv.innerHTML = "";

        let daysOfTheWeek = ["Monday", "Tuesday", "Wednsday", "Thursday", "Friday", "Saturday", "Sunday"];
        var today = new Date();
        var firstOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
        var lastOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);
        var n = lastOfMonth.getDate();
        var start = (firstOfMonth.getDay()-1)%7;       
        daysOfTheWeek.forEach((x,index)=>{
            let dayDiv = document.createElement("div");
            dayDiv.className = "dayDiv";
            this.calendarDiv.appendChild(dayDiv);
            let dayName = document.createElement("label");
            dayName.innerHTML = x;
            dayDiv.appendChild(dayName);
            var currentDay = index - start;
            var daysEntered = 0;
            while(currentDay<n || daysEntered < 6){
                let dateDiv = document.createElement("div");
                dateDiv.className = "dateDiv";
                dateDiv.id = `day${currentDay}`;
                dayDiv.appendChild(dateDiv);
                let calendarNumber = document.createElement("label");
                if(currentDay>=0 && currentDay < n){                 
                    calendarNumber.className = "dateLabel";
                    calendarNumber.innerHTML = currentDay + 1;                    
                }
                else{
                    calendarNumber.innerHTML = "&nbsp;";
                }
                dateDiv.appendChild(calendarNumber);
                daysEntered++;
                currentDay += 7;
            }
        });
        console.log(reservations);
        reservations.forEach(reservation =>
        {
            console.log(reservation);
            let dayDiv = document.querySelector(`#day${reservation.ArrivalDate - 1}`);
            let roomDiv = document.querySelector(`#room${reservation.Room}`);                              
            dayDiv.style.backgroundColor = roomDiv.bckCol;
            dayDiv.addEventListener("click", function(){ alert(`Name: ${reservation.Name}\n Surname ${reservation.Surname} \n RoomNo: ${reservation.Room}`);});

        });
    }

    drawList(divToDraw,headline, drawType,values){
        divToDraw.innerHTML = "";
        let naslov = document.createElement("h2");
        naslov.innerHTML = headline;
        divToDraw.appendChild(naslov);
        let listDiv = document.createElement("div");
        listDiv.className = "listGroup";
        divToDraw.appendChild(listDiv);
        values.forEach(value=>{
            let vrednost;
            let containerId;
            let valueOfDiv;
            if(drawType == "hotel"){                
                vrednost = value.name;
                containerId = value.name.replaceAll(" ","_");
                valueOfDiv = value.name;
            }
            else if(drawType == "room"){
                vrednost = `#${value.number}: capacity:${value.capacity}`;
                containerId =`room${value.number}`;
                valueOfDiv = value.number;                                
            }
            else{
                vrednost = `${value.name}\n${value.surname}`;
                containerId = value.idNumber;
                //valueOfDiv = value.;
            }
            let container = document.createElement("div");
            container.classList.add("listElement");
            container.classList.add("unselectedListElement");
            container.id = containerId;
            container.valueOfDiv = valueOfDiv;
            listDiv.appendChild(container);
            var containerClick = (event) => this.selectDiv(drawType, container);
            container.addEventListener("click", containerClick, false);

            let hotLabel = document.createElement("label");
            hotLabel.innerHTML = vrednost;
            container.appendChild(hotLabel);

            if(drawType == "room"){
                var x = Math.floor(Math.random() * 256);
                var y = Math.floor(Math.random() * 256);
                var z = Math.floor(Math.random() * 256);
                var bgColor = "rgb(" + x + "," + y + "," + z + ")";
                container.bckCol = bgColor;
                
                container.style.backgroundColor = bgColor;
                var light = 256 /2;
                if(x<light && y<light && z<light){
                    container.style.color = "white";
                }
                else{
                    container.style.color = "black";
                }
            }
            else if(drawType == "clients"){
                container.documentType = value.idType;             
            }
        })
    }

    selectDiv(selectType, container){
        let currentlySelected;
        if(selectType == "hotel"){
            currentlySelected = this.selectedHotel;
            this.selectedHotel = container;
            this.loadRooms(container.valueOfDiv);
            this.loadClients(container.valueOfDiv);
        }
        else if(selectType == "room"){
            currentlySelected = this.selectedRoom;
            this.selectedRoom = container;
        }
        else{
            currentlySelected = this.selectedClient;
            this.selectedClient = container;
        }

        if(currentlySelected!=null){
            currentlySelected.classList.remove("selectedDiv");
            currentlySelected.classList.add("unselectedListElement");
        }
        container.classList.remove("unselectedListElement");
        container.classList.add("selectedDiv");
    }   
    
    
    drawInput(divToDraw, listOfCreateCommands, listOfDeleteCommands){
        if(listOfCreateCommands!=null){
            listOfCreateCommands.forEach((command, index)=>{
                let groupDiv = document.createElement("div");
                groupDiv.className = "commandGroup";
                divToDraw.appendChild(groupDiv);
                let sentance = command.split(" ");
                let lastWord = sentance[sentance.length - 1];
                
                var addClick = (event) => {this.commandButtonClick(`+${lastWord}`)};
                var delClick = (event) => {this.commandButtonClick(`-${lastWord}`)};
                
                let button = document.createElement("button");
                button.innerHTML = command;
                button.addEventListener("click", addClick, false);
                groupDiv.appendChild(button);

                button = document.createElement("button");
                button.innerHTML = listOfDeleteCommands[index];
                button.addEventListener("click", delClick, false);
                groupDiv.appendChild(button);            
            })              
        }
    }

    commandButtonClick(clickType){  
        let labels;
        let types;
        let buttonText;
        let tips = null;
        let formDiv = this.inputFormDiv;        
        if(clickType.includes("+")){
            if(clickType.includes("user")){
                labels = ["Username", "Password", "Name", "Surname", "UserType"];
                types =  ["text", "password", "text", "text", "number"]; 
                tips = ["UserType (1-Manager, 2-Worker)", "Select a hotel where this user will work."];            
            }
            else if(clickType.includes("hotel")){
               labels = ["Name", "Address", "City", "Rating"];
               types =  ["text", "text", "text", "number"];
              
            }
            else if(clickType.includes("room")){
                labels = ["Room number", "Capacity", "Open"];
                types =  ["number", "number", "checkbox"];
                tips = ["Select a hotel to add the room to!"];
            }            
            else{
                formDiv = this.bookroomFormDiv;
                labels = ["Arrival Date", "Departure Date", "Client Name", "Client Surname", "Passport", "Document Number", "Phone number"];
                types =  ["date", "date", "text", "text", "checkbox", "number", "tel"];
                tips = ["Select a room!","Leave client fields empty if a client is selected."]
            }
            buttonText = "Add";            
        }
        else{
            if(clickType.includes("user")){
                labels = ["Username"];
                types = ["text"];
            }
            else if(clickType.includes("hotel")){
                tips = ["Select a hotel to remove!"];
             }
             else if(clickType.includes("room")){
                tips = ["Select a room to remove!"];
             }  
            
            else if(clickType.includes("RESERVATION")){                
                formDiv = this.bookroomFormDiv; 
                labels = ["Arrival Date"];
                types =  ["date"];
                tips = ["Select a room!"];
            }
            buttonText = "Delete"
        }
        this.openForm(formDiv, labels, types, buttonText, clickType, tips);
    }

    openForm(formDiv, labels, types, buttonText, functionText, tips){
        formDiv.innerHTML = "";
        let fieldsDiv = document.createElement("div");
        fieldsDiv.className = "inputFieldsDiv";
        formDiv.appendChild(fieldsDiv);

        
        if(labels!=null){
            let labelText;
            let inputField;
            let inputFieldDiv;
            labels.forEach((label, index) => {
                inputFieldDiv = document.createElement("div");
                inputFieldDiv.className = "inputClass";
                formDiv.appendChild(inputFieldDiv);
                labelText = document.createElement("label");
                labelText.innerHTML = label;
                labelText.className = "inputLabelClass";
                inputFieldDiv.appendChild(labelText);

                inputField = document.createElement("input");
                inputField.type = types[index];            
                inputField.id = `id${label.replaceAll(" ", "_")}`;
                inputField.className = "inputFieldsClass"
                inputFieldDiv.appendChild(inputField);
            });
        }

        if(tips!=null){
            let tipLable;
            tips.forEach(tip=>{
                tipLable = document.createElement("label");
                tipLable.innerHTML = tip;
                tipLable.className = "tips"
                formDiv.appendChild(tipLable);
            })
        }
        
        let doneButton = document.createElement("button");
        doneButton.innerHTML = buttonText;  
        var doneClick = (event) => {this.doneForm(functionText, labels);}
        doneButton.addEventListener("click",doneClick, false);
        formDiv.appendChild(doneButton);

        
    }

    doneForm(clickType, labels){
        console.log(labels);
        let values;
        if(labels!=null){
                values = labels;
                labels.forEach((label, index) => {
                let element = document.querySelector(`#id${label.replaceAll(" ", "_")}`);
                if(element!=null){
                    values[index] = element.value;
                }
                else values[index] = null;
            });
        }
        if(clickType.includes("+")){
            if(clickType.includes("user")){
                this.addUser(values[0], values[1], values[2], values[3], values[4]);
            }
            else if(clickType.includes("hotel")){                             
                this.addHotel(values[0], values[1], values[2], values[3]);
            }
            else if(clickType.includes("room")){
                this.addRoom(values[0], values[1], values[2], values[3],values[4]);
            }
            else{ //Reservation
                this.addReservation(values[0], values[1], values[2], values[3], (values[4]?0:1), values[5], values[6]);            
            }           
        }
        else{
            if(clickType.includes("user")){
                this.deleteUser(values[0]);
            }      
            else if(clickType.includes("hotel")){
               this.deleteHotel();
            }
            else if(clickType.includes("room")){
                this.deleteRoom();
            }
            else{
               this.deleteReservation(values[0]);
            }
        }
    }


    async addHotel(name, address, city, rating){
        console.log("Postavlja se hotel");
        let hotel = {"name": name, "address": address, "city": city, "rating" :rating};
        let responce = await fetch(`https://localhost:5001/HotelManaging/AddNewHotel/${this.user.id}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'POST',
        body: JSON.stringify(hotel)});
        if(responce.ok){            
            this.loadHotels();
        }   
        else{
            responce.json().then(p=>{
                alert(p.message);
            });
        }    
    }    

    async deleteHotel(){                     
        if(this.selectedHotel!=null){
            await fetch(`https://localhost:5001/HotelManaging/RemoveHotel/${this.selectedHotel.valueOfDiv}`, {
            headers: {
                'Access-Token': 'token'
            },
            method:'DELETE'}).then(data=>{
                if(data.ok){
                    this.selectedHotel = null;
                    this.drawAdmin();                   
                }
                else{
                    alert("Deleting failed!");
                    console.log(data);
                }
            }).catch(err=>{
                console.log(err);
            });           
        }
        else{
            alert("Select a hotel to remove");
        }
    }


    async addRoom(roomNumber, capacity, rating, roomType, open){
        if(this.selectedHotel !=null){
            console.log("Postavlja se hotel");
            let room = {"roomNumber": roomNumber, "capacity": capacity, "rating" :rating, "roomType" :roomType, "open" :open};
            let responce = await fetch(`https://localhost:5001/Room/AddNewRoom/${this.selectedHotel.valueOfDiv}`, {
            headers: {
                "Content-Type": "application/json"
            },
            method:'POST',
            body: JSON.stringify(room)}).then(data=>{
                if(data.ok){
                    this.inputFormDiv.innerHTML = "";
                    alert("New room added!");
                    this.loadRooms(this.selectedHotel.valueOfDiv);
                }   
                else{
                    data.json().then(resp=>{
                        alert(resp.message);
                    });                    
                    
                }
            }).catch(err=>{
                console.log(err);
            });
               
        }
        else{
            alert("Izaberite hotel!");
        }
    }

    async deleteRoom(){
        console.log("Deleting room");    
        if(this.selectedHotel != null && this.selectedRoom!=null){         
            await fetch(`https://localhost:5001/Room/RemoveRoom/${this.selectedHotel.valueOfDiv}/${this.selectedRoom.valueOfDiv}`, {
            headers: {
                "Content-Type": "application/json"
            },
            method:'DELETE'}).then(data=>{
                if(data.ok){
                    this.loadRooms(this.selectedHotel.valueOfDiv);
                    this.loadClients(this.selectedHotel.valueOfDiv);
                }
                else{

                }
            }).catch(err=>{
                console.log(err);
            });           
        }
        else{
            alert("Izaberite hotel i sobu!");
        }
    }

    async addReservation(dateOfArrival, dateOfDeparture, ime, prezime, documentType, documentIdNumber, phoneNumber){
        if(this.selectedHotel != null && this.selectedRoom!=null){
            let userReady = false;
            let clientIdNumber;
            let clientIdType;
            if(ime != ""){
                console.log("Postavlja se klient");
                let client = {"ime": ime, "prezime": prezime, "documentType" :documentType, "documentIdNumber" :documentIdNumber, "phoneNumber" :phoneNumber};
                console.log(client);
                await fetch(`https://localhost:5001/Client/AddNewClient/${this.selectedHotel.valueOfDiv}`, {
                headers: {
                    "Content-Type": "application/json"
                },
                method:'POST',
                body: JSON.stringify(client)}).then(data=>{
                    if(data.ok){
                        userReady = true;
                        clientIdNumber = documentIdNumber;
                        clientIdType = documentType;
                    }   
                    else{                    
                        data.json().then(resp=>{
                            console.log(resp);
                            alert(resp.message);
                        });                    
                        
                    }
                }).catch(err=>{
                    console.log(err);
                });               
            }
            else if(!userReady && this.selectedClient != null){
                clientIdNumber = this.selectedClient.id;
                clientIdType = this.selectedClient.documentType;
                userReady = true;
            }
            

            if(userReady){
                await fetch(`https://localhost:5001/Booking/BookARoom/${dateOfArrival}/${dateOfDeparture}/${this.selectedHotel.valueOfDiv}/${this.selectedRoom.valueOfDiv}/${clientIdType}/${clientIdNumber}`, {
                    headers: {
                        "Content-Type": "application/json"
                    },
                    method:'POST'}).then(data=>{
                        if(data.ok){
                            this.bookroomFormDiv.innerHTML = "";
                            this.loadRooms(this.selectedHotel.valueOfDiv);
                            this.loadClients(this.selectedHotel.valueOfDiv);
                            alert("New reservation made");                        
                        }   
                        else{                                      
                            data.json().then(resp=>{
                                alert(resp.message);
                            });  
                        }
                    }).catch(err=>{
                        console.log(err);
                    });
            }
            else{
                alert("Select client or add client information");
            }
        }
        else{
            alert("Select hotel and room!");
        }
    }
        

    async deleteReservation(dateOfArrival){    
        if(this.selectedHotel != null && this.selectedRoom!=null){         
            await fetch(`https://localhost:5001/Booking/CancelAReservation/${dateOfArrival}/${this.selectedHotel.valueOfDiv}/${this.selectedRoom.valueOfDiv}`, {
            headers: {
                "Content-Type": "application/json"
            },
            method:'DELETE'}).then(data=>{
                if(data.ok){
                    this.loadCalendar(this.selectedHotel.valueOfDiv);
                }
                else{
                   console.log(data);
                   alert("Can't cancel this reservation");
                }
            }).catch(err=>{
                console.log(err);
            });           
        }
        else{
            alert("Izaberite hotel i sobu!");
        }
    }


    async addUser(username, password, name, surname, userType){
        if(this.selectedHotel!=null){
            let newUser = {"username": username, "password": password, "ime": name, "prezime": surname};
            console.log(newUser);
            console.log(this.selectedHotel.valueOfDiv);
            await fetch(`https://localhost:5001/User/AddUser/${this.selectedHotel.valueOfDiv}/${userType}`, {
            headers: {
                "Content-Type": "application/json"
            },
            method:'POST',
            body: JSON.stringify(newUser)}).then(responce =>{
                if(responce.ok){            
                    alert("New user added. ")
                }   
                else{
                    console.log(responce);
                    responce.json().then(p=>{
                        console.log(p.errors);
                        alert(p.message);
                    });
                }
            });
                
        }
        else{
            alert("Select a hotel to employ the user to!");
        }
    }   


    async deleteUser(username){    
        
        await fetch(`https://localhost:5001/User/RemoveUser/${username}`, {
        headers: {
            "Content-Type": "application/json"
        },
        method:'DELETE'}).then(data=>{
            if(data.ok){
                alert("User was removed.");
            }
            else{
                alert("User wasn't removed.");
            }
        }).catch(err=>{
            console.log(err);
        });        
    }
}