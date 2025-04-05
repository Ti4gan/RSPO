using System;
using System.Collections.Generic;
using System.Linq;

namespace Pract1
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                List<Client> clients = new List<Client>
            {
                new Client(1, "Иванов Иван Иванович", 123456789, true),
                new Client(2, "Петров Петр Петрович", 987654321)
            };

                List<Item> items = new List<Item>
            {
                new Item("UT-75X", "Товар 1", 100),
                new Item("A1", "Товар 2", 200),
                new Item("B2", "Товар 3", 300),
                new Item("C3", "Товар 4", 400),
                new Item("D4", "Товар 5", 500),
                new Item("E5", "Товар 6", 600),
                new Item("F6", "Товар 7", 700)
            };

                List<IDiscount> discounts = new List<IDiscount>
            {
                new Discount1000(),
                new Discount1500(),
                new PrivilegedClient(),
                new Discount1500AndPrivileged(),
                new HolidayDiscount(),
                new ArticulDiscount()
            };

                Console.Write("Введите номер заказа: ");
                int orderNumber;
                while (!int.TryParse(Console.ReadLine(), out orderNumber) || orderNumber <= 0)
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите положительное число.");
                }

                Console.Write("Введите дату поступления (гггг-мм-дд): ");
                DateTime creationDate;
                while (!DateTime.TryParse(Console.ReadLine(), out creationDate))
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите дату в формате гггг-мм-дд.");
                }

                Console.Write("Введите адрес доставки: ");
                string address = Console.ReadLine();

                Console.Write("Заказ срочный? (да/нет): ");
                bool expressDelivery = Console.ReadLine().ToLower() == "да";

                Console.WriteLine("Выберите клиента:");
                for (int i = 0; i < clients.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {clients[i].FullName}");
                }
                int clientIndex;
                while (!int.TryParse(Console.ReadLine(), out clientIndex) || clientIndex <= 0 || clientIndex > clients.Count)
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите номер из списка.");
                }
                Client selectedClient = clients[clientIndex - 1];

                Order order = new Order(orderNumber, creationDate, address, expressDelivery, selectedClient);

                while (true)
                {
                    Console.WriteLine("Выберите товар:");
                    for (int i = 0; i < items.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {items[i].Name} - {items[i].UnitPrice} руб.");
                    }

                    int itemIndex;
                    while (!int.TryParse(Console.ReadLine(), out itemIndex) || itemIndex <= 0 || itemIndex > items.Count)
                    {
                        Console.WriteLine("Некорректный ввод. Пожалуйста, введите номер из списка.");
                    }

                    Item selectedItem = items[itemIndex - 1];

                    Console.Write("Введите количество: ");
                    int quantity;
                    while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Некорректный ввод. Пожалуйста, введите положительное число.");
                    }

                    order.AddOrderLine(selectedItem, quantity);

                    Console.Write("Добавить еще товар? (да/нет): ");
                    string response = Console.ReadLine().ToLower();
                    if (response != "да")
                    {
                        break;
                    }
                }
                order.Confirm();
                Console.WriteLine("Заказ подтвержден и переведен в состояние 'В обработке'.");
                var applicableDiscounts = order.GetApplicableDiscounts(discounts);
                if (applicableDiscounts.Any())
                {
                    Console.WriteLine("Доступные скидки:");
                    for (int i = 0; i < applicableDiscounts.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {applicableDiscounts[i].Description}");
                    }

                    Console.Write("Выберите скидку (укажите номер): ");
                    int discountIndex;
                    while (!int.TryParse(Console.ReadLine(), out discountIndex) || discountIndex <= 0 || discountIndex > applicableDiscounts.Count)
                    {
                        Console.WriteLine("Некорректный ввод. Пожалуйста, введите номер из списка.");
                    }

                    IDiscount selectedDiscount = applicableDiscounts[discountIndex - 1];
                    decimal totalCostWithDiscount = order.ApplySelectedDiscount(selectedDiscount);
                    Console.WriteLine($"Общая стоимость заказа с учетом скидки: {totalCostWithDiscount} руб.");
                }
                else
                {
                    Console.WriteLine("Нет доступных скидок.");
                }

                order.StartDelivery();
                Console.WriteLine("Заказ передан в доставку.");
                order.CompleteDelivery();
                Console.WriteLine("Заказ доставлен.");
                Console.WriteLine($"Статус заказа: {order.Status}");
            }
        }

        public class Discount1000 : IDiscount
        {
            public string Description => "5% скидка (стоимость заказа превышает 1000)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                return totalCost > 1000 ? totalCost * 0.95m : totalCost;
            }
        }

        public class Discount1500 : IDiscount
        {
            public string Description => "10% скидка (стоимость заказа превышает 1500)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                return totalCost > 1500 ? totalCost * 0.90m : totalCost;
            }
        }

        public class PrivilegedClient : IDiscount
        {
            public string Description => "7% скидка (клиент привилегированный)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                return order.Client.Privileged ? totalCost * 0.93m : totalCost;
            }
        }

        public class Discount1500AndPrivileged : IDiscount
        {
            public string Description => "15% скидка (стоимость заказа превышает 1500 и клиент привилегированный)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                return (totalCost > 1500 && order.Client.Privileged) ? totalCost * 0.85m : totalCost;
            }
        }

        public class HolidayDiscount : IDiscount
        {
            public string Description => "12% скидка (заказ сформирован в период с 25 декабря по 7 января)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                DateTime orderDate = order.CreationDate;
                bool isHolidayPeriod = (orderDate.Month == 12 && orderDate.Day >= 25) || (orderDate.Month == 1 && orderDate.Day <= 7);
                return isHolidayPeriod ? totalCost * 0.88m : totalCost;
            }
        }

        public class ArticulDiscount : IDiscount
        {
            public string Description => "4% скидка (товар с артикулом UT-75X)";
            public decimal ApplyDiscount(Order order, decimal totalCost)
            {
                bool hasSpecificItem = order.OrderLines.Any(line => line.Item.Article == "UT-75X");
                return hasSpecificItem ? totalCost * 0.96m : totalCost;
            }
        }
    }
}