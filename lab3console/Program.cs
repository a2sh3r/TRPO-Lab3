using System;
using System.Collections.Generic;

namespace VehicleNamespace
{
    // Делегаты
    public delegate void ChargeAction(IElectricVehicle vehicle);
    public delegate void RefuelAction(IFuelVehicle vehicle);

    // Интерфейсы
    public interface IVehicle
    {
        void Drive();
    }

    public interface IMaintenance
    {
        void Repair();
    }

    public interface IElectricVehicle
    {
        void Charge();
    }

    public interface IFuelVehicle
    {
        void Refuel();
    }

    // Базовый класс
    public abstract class Vehicle
    {
        public string Model { get; set; }
        public int Year { get; set; }
    }

    // Второй уровень иерархии
    public abstract class PassengerVehicle : Vehicle, IVehicle
    {
        public int PassengerCapacity { get; set; }

        public abstract void Drive();
    }

    public abstract class CommercialVehicle : Vehicle, IVehicle, IMaintenance
    {
        public int CargoCapacity { get; set; }

        public abstract void Drive();
        public abstract void Repair();
    }

    // Третий уровень иерархии
    public class Car : PassengerVehicle
    {
        public override void Drive()
        {
            Console.WriteLine($"The {Model} car is driving.");
        }
    }

    public class Truck : CommercialVehicle, IFuelVehicle
    {
        public override void Drive()
        {
            Console.WriteLine($"The {Model} truck is driving.");
        }

        public override void Repair()
        {
            Console.WriteLine($"The {Model} truck is being repaired.");
        }

        public void Refuel()
        {
            Console.WriteLine($"The {Model} truck is refueling.");
        }
    }

    public class ElectricCar : PassengerVehicle, IElectricVehicle
    {
        public override void Drive()
        {
            Console.WriteLine($"The {Model} electric car is driving.");
        }

        public void Charge()
        {
            Console.WriteLine($"The {Model} electric car is charging.");
        }
    }

    public class ElectricBus : PassengerVehicle, IElectricVehicle
    {
        public override void Drive()
        {
            Console.WriteLine($"The {Model} electric bus is driving.");
        }

        public void Charge()
        {
            Console.WriteLine($"The {Model} electric bus is charging.");
        }
    }

    // Четвертый уровень иерархии
    public class LuxuryCar : Car
    {
        public string Features { get; set; }
    }

    public class DeliveryTruck : Truck
    {
        public string CargoType { get; set; }
    }

    public class PassengerElectricBus : ElectricBus
    {
        public bool HasWifi { get; set; }
    }

    public class CargoElectricVan : ElectricCar, IMaintenance
    {
        public int CargoCapacity { get; set; }

        public void Repair()
        {
            Console.WriteLine($"The {Model} electric van is being repaired.");
        }
    }

    // Класс для автопарка
    public class CarPark
    {
        public event ChargeAction VehicleCharged;
        public event RefuelAction VehicleRefueled;

        private List<Vehicle> vehicles;

        public CarPark()
        {
            vehicles = new List<Vehicle>();
        }

        public List<Vehicle> GetVehicles()
        {
            return vehicles;
        }

        private void LogToFile(string message)
        {
            string logFilePath = "carpark_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        public void AddVehicle(Vehicle vehicle)
        {
            vehicles.Add(vehicle);
            LogToFile($"Added {vehicle.Model}");
        }

        public void DriveAllVehicles(Action<IVehicle> driveAction)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle is IVehicle iv)
                {
                    driveAction(iv);
                    LogToFile($"Driving {vehicle.Model}");
                }
                else
                {
                    Console.WriteLine($"The {vehicle.Model} cannot be driven.");
                    LogToFile($"The {vehicle.Model} cannot be driven.");
                }
            }
        }

        // Обработка ошибок
        public void RepairAllVehicles(Action<IMaintenance> repairAction)
        {
            foreach (var vehicle in vehicles)
            {
                try
                {
                    if (vehicle is IMaintenance im)
                    {
                        repairAction(im);
                        LogToFile($"Repairing {vehicle.Model}");
                    }
                    else
                    {
                        Console.WriteLine($"The {vehicle.Model} cannot be repaired.");
                        LogToFile($"The {vehicle.Model} cannot be repaired.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while repairing {vehicle.Model}: {ex.Message}");
                    LogToFile($"An error occurred while repairing {vehicle.Model}: {ex.Message}");
                }
            }
        }

        public Vehicle GetVehicleByIndex(int index)
        {
            try
            {
                return vehicles[index];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Error: {ex.Message}. Please provide a valid index.");
                return null;
            }
        }

        public void ListVehicles()
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                Console.WriteLine($"{i}: {vehicles[i].Model} ({vehicles[i].Year}) - {vehicles[i].GetType().Name}");
            }
        }

        public void ChargeAllVehicles(ChargeAction chargeAction)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle is IElectricVehicle ev)
                {
                    chargeAction(ev);
                    LogToFile($"Charging {vehicle.Model}");

                    VehicleCharged?.Invoke(ev);
                }
            }
        }

        public void RefuelAllVehicles(RefuelAction refuelAction)
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle is IFuelVehicle fv)
                {
                    refuelAction(fv);
                    LogToFile($"Refueling {vehicle.Model}");

                    VehicleRefueled?.Invoke(fv);
                }
            }
        }
    }

    // Пример использования
    class Program
    {
        static void Main(string[] args)
        {
            var carPark = new CarPark();

            var vehicleTypes = new Dictionary<string, Type>
            {
                { "1", typeof(LuxuryCar) },
                { "2", typeof(DeliveryTruck) },
                { "3", typeof(ElectricCar) },
                { "4", typeof(PassengerElectricBus) },
                { "5", typeof(CargoElectricVan) }

            };

            carPark.AddVehicle(new LuxuryCar { Model = "Sedan", Year = 2020, Features = "Leather seats" });
            carPark.AddVehicle(new DeliveryTruck { Model = "T8", Year = 2018, CargoType = "Furniture" });
            carPark.AddVehicle(new ElectricCar { Model = "Tesla", Year = 2021 });
            carPark.AddVehicle(new PassengerElectricBus { Model = "BYD", Year = 2022, HasWifi = true });
            carPark.AddVehicle(new CargoElectricVan { Model = "e-NV200", Year = 2023, CargoCapacity = 800 });

            Console.WriteLine("Welcome to the Car Park!");
            Console.WriteLine("Press ESC to exit.");
            Console.WriteLine("Press 1 to list all vehicles.");
            Console.WriteLine("Press 2 to drive a vehicle.");
            Console.WriteLine("Press 3 to repair a vehicle.");
            Console.WriteLine("Press 4 to add a vehicle.");
            Console.WriteLine("Press 5 to charge all electic vehicles.");
            Console.WriteLine("Press 6 to refuel all non-electric vehicles.");

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        carPark.ListVehicles();
                        break;
                    case ConsoleKey.D2:
                        carPark.ListVehicles();
                        Console.WriteLine("Enter the index of the vehicle to drive:");
                        string userInput = Console.ReadLine();
                        if (int.TryParse(userInput, out int index))
                        {
                            var vehicle = carPark.GetVehicleByIndex(index);
                            if (vehicle is IVehicle drivableVehicle)
                            {
                                drivableVehicle.Drive();
                            }
                            else
                            {
                                Console.WriteLine("The selected vehicle cannot be driven.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid index. Please enter a valid number.");
                        }
                        break;
                    case ConsoleKey.D3:
                        carPark.ListVehicles();
                        Console.WriteLine("Enter the index of the vehicle to repair:");
                        userInput = Console.ReadLine();
                        if (int.TryParse(userInput, out index))
                        {
                            var vehicle = carPark.GetVehicleByIndex(index);
                            if (vehicle is IMaintenance maintenanceVehicle)
                            {
                                maintenanceVehicle.Repair();
                            }
                            else
                            {
                                Console.WriteLine("The selected vehicle cannot be repaired.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid index. Please enter a valid number.");
                        }
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine("Choose the type of vehicle to add:");
                        Console.WriteLine("1. LuxuryCar");
                        Console.WriteLine("2. DeliveryTruck");
                        Console.WriteLine("3. ElectricCar");
                        Console.WriteLine("4. PassengerElectricBus");
                        Console.WriteLine("5. CargoElectricVan");
                        string userChoice = Console.ReadLine();
                        if (vehicleTypes.TryGetValue(userChoice, out Type vehicleType))
                        {
                            Console.WriteLine("Enter the model of the vehicle:");
                            string model = Console.ReadLine();
                            Console.WriteLine("Enter the year of the vehicle:");
                            string yearInput = Console.ReadLine();
                            if (int.TryParse(yearInput, out int year))
                            {
                                try
                                {
                                    // Создаем экземпляр класса и устанавливаем свойства
                                    Vehicle newVehicle = (Vehicle)Activator.CreateInstance(vehicleType);
                                    newVehicle.Model = model;
                                    newVehicle.Year = year;

                                    // Добавляем новую машину в автопарк
                                    carPark.AddVehicle(newVehicle);
                                    Console.WriteLine($"The {vehicleType.Name} has been added to the car park.");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"An error occurred while adding the vehicle: {ex.Message}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid year. Please enter a valid number.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid vehicle type. Please choose a valid option.");
                        }
                        break;
                    case ConsoleKey.D5:
                        carPark.ChargeAllVehicles((IElectricVehicle ev) => ev.Charge());
                        break;
                    case ConsoleKey.D6:
                        carPark.RefuelAllVehicles((IFuelVehicle fv) => fv.Refuel());
                        break;
                    case ConsoleKey.Escape:
                        Console.WriteLine("Exiting the program...");
                        return; // Выход из цикла и завершение приложения
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            }
        }
    }
}