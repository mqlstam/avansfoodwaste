using Avans.FoodWaste.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avans.FoodWaste.Infrastructure.Data
{
    public class FoodWasteDataSeeder
    {
        public static void SeedData(FoodWasteDbContext context)
        {
            if (context.Cafeterias.Any())
            {
                return; // Skip seeding if data already exists
            }

            // Sample Cafeterias
            var cafeteria1 = new Cafeteria
                { City = "Breda", LocationIdentifier = "1a", HotMealsAvailable = true, OperatingHours = "8:00-17:00" };
            var cafeteria2 = new Cafeteria
            {
                City = "Tilburg", LocationIdentifier = "2b", HotMealsAvailable = false, OperatingHours = "9:00-16:00"
            };
            var cafeteria3 = new Cafeteria
            {
                City = "Den Bosch", LocationIdentifier = "3c", HotMealsAvailable = true, OperatingHours = "8:30-17:30"
            };

            context.Cafeterias.AddRange(cafeteria1, cafeteria2, cafeteria3);
            context.SaveChanges();

            // Sample Staff
            var staff1 = new CafeteriaStaff { Name = "John Doe", EmployeeNumber = "12345", Cafeteria = cafeteria1 };
            var staff2 = new CafeteriaStaff { Name = "Jane Doe", EmployeeNumber = "67890", Cafeteria = cafeteria2 };
            var staff3 = new CafeteriaStaff { Name = "Peter Jones", EmployeeNumber = "13579", Cafeteria = cafeteria3 };

            context.CafeteriaStaff.AddRange(staff1, staff2, staff3);
            context.SaveChanges();

            // Sample Products
            var product1 = new Product { Name = "Baguette", ContainsAlcohol = false, ProductType = "Bread" };
            var product2 = new Product { Name = "Croissant", ContainsAlcohol = false, ProductType = "Bread" };
            var product3 = new Product { Name = "Salad", ContainsAlcohol = false, ProductType = "Other" };
            var product4 = new Product { Name = "Pasta", ContainsAlcohol = false, ProductType = "HotDinner" };
            var product5 = new Product { Name = "Beer", ContainsAlcohol = true, ProductType = "Drinks" };

            context.Products.AddRange(product1, product2, product3, product4, product5);
            context.SaveChanges();

            // Sample Students
            var student1 = new Student
            {
                Name = "Alice",
                DateOfBirth = new DateTime(2005, 5, 10), 
                StudentNumber = "1234567",
                Email = "alice@example.com",
                StudyCity = "Breda",
                PhoneNumber = "1234567890"
            };
            var student2 = new Student
            {
                Name = "Bob",
                DateOfBirth = new DateTime(2004, 11, 20),
                StudentNumber = "7654321",
                Email = "bob@example.com",
                StudyCity = "Tilburg",
                PhoneNumber = "9876543210"
            };
            var student3 = new Student
            {
                Name = "Charlie",
                DateOfBirth = new DateTime(2007, 2, 15),
                StudentNumber = "9876543",
                Email = "charlie@example.com",
                StudyCity = "Den Bosch",
                PhoneNumber = "5551234567"
            };

            context.Students.AddRange(student1, student2, student3);
            context.SaveChanges();

            // Sample Packages (linked to Cafeterias and Products)
            var package1 = new Package
            {
                Name = "Leftover Bread",
                ExampleProductIds = new List<int> { product1.Id, product2.Id },
                PickupDateTime = DateTime.Now.AddDays(1),
                LatestPickupTime = DateTime.Now.AddDays(1).AddHours(1),
                IsAdultPackage = false,
                Price = 2.50m,
                MealType = MealType.Bread,
                ReservationStatus = ReservationStatus.Reserved, // Package 1 is reserved
                NoShowStatus = NoShowStatus.None,
                Cafeteria = cafeteria1,
                ReservedById = student1.Id // Student 1 has reserved package 1
            };

            var package2 = new Package
            {
                Name = "Hot Meal Deal",
                ExampleProductIds = new List<int> { product4.Id, product3.Id },
                PickupDateTime = DateTime.Now.AddDays(2),
                LatestPickupTime = DateTime.Now.AddDays(2).AddHours(1),
                IsAdultPackage = false,
                Price = 5.00m,
                MealType = MealType.HotDinner,
                ReservationStatus = ReservationStatus.Available,
                NoShowStatus = NoShowStatus.None,
                Cafeteria = cafeteria3
            };

            var package3 = new Package
            {
                Name = "Drinks Combo",
                ExampleProductIds = new List<int> { product5.Id },
                PickupDateTime = DateTime.Now.AddDays(1),
                LatestPickupTime = DateTime.Now.AddDays(1).AddHours(1),
                IsAdultPackage = true,
                Price = 3.00m,
                MealType = MealType.Drinks,
                ReservationStatus = ReservationStatus.Available,
                NoShowStatus = NoShowStatus.None,
                Cafeteria = cafeteria2
            };

            context.Packages.AddRange(package1, package2, package3);
            context.SaveChanges();

            // Sample Reservations (matching the pre-reserved package)
            var reservation1 = new Reservation
            {
                StudentId = student1.Id,  // Student 1 reserved Package 1
                PackageId = package1.Id,  // Package 1
                ReservationDate = DateTime.UtcNow.AddDays(-1)
            };

            // Now package 1 is reserved by student 1, 
            // and student 1 has a reservation for today.

            context.Reservations.AddRange(reservation1);
            context.SaveChanges();
        }
    }
}