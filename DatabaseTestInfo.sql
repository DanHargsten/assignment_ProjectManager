DELETE FROM ProjectEmployees;
DELETE FROM Employees;
DELETE FROM Projects;
DELETE FROM Customers;

DBCC CHECKIDENT ('Employees', RESEED, 0);
DBCC CHECKIDENT ('Projects', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);

INSERT INTO Employees(FirstName, LastName, Email, Phone, Role)
VALUES ('Giang', 'Hubbard', 'testone@domain.com', '', 1);
INSERT INTO Employees(FirstName, LastName, Email, Phone, Role)
VALUES ('Chipiliro', 'Accardo', 'testtwo@domain.com', '', 2);
INSERT INTO Employees(FirstName, LastName, Email, Phone, Role)
VALUES ('Kiran', 'Skylar', 'testthree@domain.com', '', 3);   

INSERT INTO Customers(Name, Email, PhoneNumber)
VALUES ('Ryggskott Corp.', 'customerone@domain.com', '');
INSERT INTO Customers(Name, Email, PhoneNumber)
VALUES ('Rackartygarna Inc.', 'customertwo@domain.com', '');
INSERT INTO Customers(Name, Email, PhoneNumber)
VALUES ('Blästrad AB', 'customerthree@domain.com', '');

INSERT INTO Projects(Title, Description, StartDate, EndDate, CreatedDate, Status, CustomerId)
VALUES ('Test Tactics', 'Número de prueba ett', '', '', '2000-01-01', 2, 1);
INSERT INTO Projects(Title, Description, StartDate, EndDate, CreatedDate, Status, CustomerId)
VALUES ('Trial Triumph', 'Ett testprojekt för databasen', '', '', '2000-01-01', 2, 2);
INSERT INTO Projects(Title, Description, StartDate, EndDate, CreatedDate, Status, CustomerId)
VALUES ('Evaluation Enclave', 'Das beste Projekt', '', '', '2000-01-01', 2, 3);

INSERT INTO ProjectEmployees (ProjectId, EmployeeId)
VALUES (1, 1);
INSERT INTO ProjectEmployees (ProjectId, EmployeeId)
VALUES (2, 2);
INSERT INTO ProjectEmployees (ProjectId, EmployeeId)
VALUES (3, 1);
INSERT INTO ProjectEmployees (ProjectId, EmployeeId)
VALUES (3, 2);