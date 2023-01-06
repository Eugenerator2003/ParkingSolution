async function GetAllCars() {
    const response = await fetch("https://localhost:7254/api/Cars", {
        method: "Get",
        headers: { "Accept" : "application/json" }
    });

    if (response.ok === true) {
        const cars = await response.json();
        var array = cars.$values
        let rows = document.querySelector("tbody")
        array.forEach(car => {
            rows.append(row(car));
        });
    }
}

async function GetAllMarks() {
	var selectList = carForm.carMark;
	const response = await fetch("https://localhost:7254/api/CarMarks", {
		method: "GET",
		headers: { "Accept": "application/json" }
	});
	if (response.ok === true) {
		const carMarks = await response.json();
		carMarks.forEach(carMark => {
			var option = document.createElement("option");
			option.text = carMark.name;
			option.value = parseInt(carMark.id);
			selectList.append(option)
		});
	}
}

async function GetAllOwners() {
	var selectList = carForm.owner;
	const response = await fetch("https://localhost:7254/api/Owners", {
		method: "GET",
		headers: { "Accept": "application/json" }
	});
	if (response.ok === true) {
		const owners = await response.json();
		owners.forEach(owner => {
			var option = document.createElement("option");
			option.text = owner.fullname;
			option.value = parseInt(owner.id);
			selectList.append(option);
		});
	}
}


async function DeleteCarById(id) {
    const response = await fetch("https://localhost:7254/api/Cars/" + id, {
        method: "DELETE",
        headers: { "Accept": "application/json" }
    });

    if (response.ok === true) {
        const film = await response.json();
        alert("Success");
        document.querySelector("tr[data-rowid='" + film.id + "']").remove();
    }
}