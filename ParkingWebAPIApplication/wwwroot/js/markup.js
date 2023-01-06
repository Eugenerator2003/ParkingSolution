//Функция для создания строки таблицы
function row(car) {
	const tr = document.createElement("tr");
	tr.setAttribute("data-rowid", car.id);

	const idTd = document.createElement("td");
	idTd.append(car.id);
	tr.append(idTd);

	const MarkTd = document.createElement("td");
	MarkTd.append(car.carMark.name);
	tr.append(MarkTd);

	const ownerTd = document.createElement("td");
	ownerTd.append(car.owner.fullname);
	tr.append(ownerTd);

	const numberTd = document.createElement("td");
	numberTd.append(car.number);
	tr.append(numberTd)

	const linksTd = document.createElement("td");

	const linkForEdit = document.createElement("a");
	linkForEdit.setAttribute("car-id", car.id);
	linkForEdit.append("Change")
	linkForEdit.addEventListener("click", event => {
		event.preventDefault();
		localStorage.setItem('id', car.id);
		window.location = "edit.html";
	});
	linksTd.append(linkForEdit);

	const linkForDelete = document.createElement("a");
	linkForDelete.setAttribute("car-id", car.id);
	linkForDelete.append("Delete");
	linkForDelete.addEventListener("click", event => {
		event.preventDefault();
		DeleteCarById(car.id)
	});
	linksTd.append(linkForDelete);

	tr.append(linksTd);
	return tr;
}