using System;
using System.Collections.Generic;
using System.Linq;
using DIO.Orders.Application.Services;
using DIO.Orders.Domain.Enums;
using DIO.Orders.Domain.Models;
using DIO.Orders.Domain.Repositories;
using DIO.Orders.Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DIO.Orders.Application.Unit.Tests.Services
{
    [TestFixture]
    internal class OrderServiceUnitTests
    {
        private const int FirstId = 1;
        private const int SecondId = 2;
        private const int ThirdId = 3;
        private const string ProductName = "Product";
        private const string CustomerName = "Customer";
        private const string OrderPromotionDescription = "Order Promotion";
        private const string ProductPromotionDescription = "Product Promotion";

        private readonly Product _product = new (ProductName, 20) { Id = FirstId };
        private readonly Product _secondProduct = new (ProductName, 30) { Id = SecondId };
        private readonly Product _thirdProduct = new (ProductName, 50) { Id = ThirdId };
        private readonly List<Product> _products = new();
        private readonly Customer _customer = new (CustomerName) { Id = FirstId };
        private readonly Customer _secondCustomer = new (CustomerName) { Id = SecondId };
        private readonly Promotion _orderPromotion = new (PromotionType.Order, null, 20, OrderPromotionDescription) { Id = FirstId };
        private readonly Promotion _productPromotion = new (PromotionType.Product, SecondId, 10, ProductPromotionDescription) { Id = SecondId };

        private readonly Random _random = new(1);

        private OrderService _service;

        private MockRepository _mockRepository;
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IPromotionRepository> _mockPromotionRepository;
        private Mock<ICustomerRepository> _mockCustomerRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockOrderRepository = _mockRepository.Create<IOrderRepository>();
            _mockProductRepository = _mockRepository.Create<IProductRepository>();
            _mockPromotionRepository = _mockRepository.Create<IPromotionRepository>();
            _mockCustomerRepository = _mockRepository.Create<ICustomerRepository>();

            _products.Add(_product);

            _service = new OrderService(_mockOrderRepository.Object, _mockProductRepository.Object, _mockCustomerRepository.Object, _mockPromotionRepository.Object);
        }

        [TestCase(typeof(IOrderService))]
        [TestCase(typeof(ServiceStorableBase<Order>))]
        public void ShouldImplementsOrInherit(Type type)
        {
            //Arrange
            //Act
            //Assert
            _service.GetType().Should().BeAssignableTo(type);
        }

        [Test]
        public void WhenAdd_WithInvalidProduct_ShouldNotCallRepository_AndReturnInvalidProductResponse()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new List<Product>());

            //Act
            var result = (ResultCodeType) _service.Add(invalidOrder);

            //Assert
            result.Should().Be(ResultCodeType.InvalidProduct);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Never);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenAdd_WithInvalidCustomer_ShouldNotCallRepository_AndReturnAsExpected()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new List<Customer>());

            //Act
            var result = (ResultCodeType)_service.Add(invalidOrder);

            //Assert
            result.Should().Be(ResultCodeType.InvalidCustomer);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenAdd_WithInvalidPromotion_ShouldNotCallRepository_AndReturnAsExpected()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer, _orderPromotion);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new[] { _customer });
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new List<Promotion>());

            //Act
            var result = (ResultCodeType)_service.Add(invalidOrder);

            //Assert
            result.Should().Be(ResultCodeType.InvalidPromotion);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenAdd_ValidOrder_ShouldCallAddOrUpdate_FromRepository_AndReturnTheIdentifier()
        {
            //Arrange
            var idInserted = _random.Next(10);
            var order = new Order(_products, _customer, _orderPromotion);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new[] { _customer });
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new[] { _orderPromotion });
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == null &&
                    o.Products.Count == _products.Count &&
                    o.Products.First().Id == _product.Id && o.Products.First().Name == _product.Name && Math.Abs(o.Products.First().Value - _product.Value) == 0 &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion.Type == _orderPromotion.Type && o.Promotion.TargetId == _orderPromotion.TargetId && o.Promotion.DiscountPercentage == _orderPromotion.DiscountPercentage && o.Promotion.Description == _orderPromotion.Description
                ))).Returns(idInserted);

            //Act
            var result = _service.Add(order);

            //Assert
            result.Should().Be(idInserted);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void WhenUpdate_WithInvalidProduct_ShouldReturnFalse()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new List<Product>());

            //Act
            var result = _service.Update(invalidOrder);

            //Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Never);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenUpdate_WithInvalidCustomer_ShouldReturnFalse()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new List<Customer>());

            //Act
            var result = _service.Update(invalidOrder);

            //Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenUpdate_WithInvalidPromotion_ShouldReturnFalse()
        {
            //Arrange
            var invalidOrder = new Order(_products, _customer, _orderPromotion);

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new[] { _customer });
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new List<Promotion>());

            //Act
            var result = _service.Update(invalidOrder);

            //Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void WhenUpdate_ValidOrder_ShouldCallAddOrUpdate_FromRepository_AndReturnsTrue()
        {
            //Arrange
            var idUpdated = _random.Next(10);
            var order = new Order(_products, _customer, _orderPromotion) { Id = idUpdated };

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new[] { _customer });
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new[] { _orderPromotion });
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == idUpdated &&
                    o.Products.Count == _products.Count &&
                    o.Products.First().Id == _product.Id && o.Products.First().Name == _product.Name && Math.Abs(o.Products.First().Value - _product.Value) == 0 &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion.Type == _orderPromotion.Type && o.Promotion.TargetId == _orderPromotion.TargetId && o.Promotion.DiscountPercentage == _orderPromotion.DiscountPercentage && o.Promotion.Description == _orderPromotion.Description
                ))).Returns(idUpdated);

            //Act
            var result = _service.Update(order);

            //Assert
            result.Should().BeTrue();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void OnCreatingOrder_WithInvalidProducts_ShouldNeverAddInRepository()
        {
            //Arrange
            var customerId = _customer.Id ?? 0;
            var productsIds = _products.Select(product => product.Id ?? 0).ToList();

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new List<Product>());

            //Act
            var result = (ResultCodeType) _service.CreateOrder(customerId, productsIds);

            //Assert
            result.Should().Be(ResultCodeType.InvalidProduct);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Never);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnCreatingOrder_WithInvalidCustomer_ShouldNeverAddInRepository()
        {
            //Arrange
            _products.Clear();
            _products.Add(_product);

            var customerId = _customer.Id ?? 0;
            var productsIds = _products.Select(product => product.Id ?? 0).ToList();
            Func<Product, bool> productFilter = null;

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products).Callback<Func<Product, bool>>(filter => productFilter = filter); ;
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new List<Customer>());

            //Act
            var result = (ResultCodeType)_service.CreateOrder(customerId, productsIds);

            //Assert
            result.Should().Be(ResultCodeType.InvalidCustomer);
            productFilter(_product).Should().BeTrue();
            productFilter(_secondProduct).Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnCreatingOrder_WithValidArguments_ShouldAddInRepository_AndReturnsTheIdentifierAdded()
        {
            //Arrange
            var idInserted = _random.Next(10);
            var customerId = _customer.Id ?? 0;
            var productsIds = _products.Select(product => product.Id ?? 0).ToList();
            Func<Customer, bool> customerFilter = null;

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(_products);
            _mockCustomerRepository.Setup(repo => repo.Get(It.IsAny<Func<Customer, bool>>())).Returns(new[] { _customer }).Callback<Func<Customer, bool>>(filter => customerFilter = filter);
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == null &&
                    o.Products.Count == _products.Count &&
                    o.Products.First().Id == _product.Id && o.Products.First().Name == _product.Name && Math.Abs(o.Products.First().Value - _product.Value) == 0 &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion == null
                ))).Returns(idInserted);

            //Act
            var result = _service.CreateOrder(customerId, productsIds);

            //Assert
            result.Should().Be(idInserted);
            customerFilter(_customer).Should().BeTrue();
            customerFilter(_secondCustomer).Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockCustomerRepository.Verify(repo => repo.Get(It.IsAny<Func<Customer, bool>>()), Times.Once);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void OnAddingProducts_WithValidProducts_ShouldAdd_UpdateTheRepository_AndReturnsAsExpected(int idUpdated, int idReturned, bool expectedResult)
        {
            //Arrange
            _products.Clear();
            _products.Add(_product);
            var order = new Order(_products, _customer) { Id = idUpdated };
            var invalidOrder = new Order(_products, _customer) { Id = SecondId };
            Func<Product, bool> productFilter = null;
            Func<Order, bool> orderFilter = null;

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new [] { _secondProduct }).Callback<Func<Product, bool>>(filter => productFilter = filter);
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new[] { order }).Callback<Func<Order, bool>>(filter => orderFilter = filter);
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == idUpdated &&
                    o.Products.Count == _products.Count &&
                    o.Products.First().Id == _product.Id && o.Products.First().Name == _product.Name && Math.Abs(o.Products.First().Value - _product.Value) == 0 &&
                    o.Products.Last().Id == _secondProduct.Id && o.Products.Last().Name == _secondProduct.Name && Math.Abs(o.Products.Last().Value - _secondProduct.Value) == 0 &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion == null
                ))).Returns(idReturned);

            //Act
            var result = _service.AddProductsTo(idUpdated, new List<int> { SecondId });

            //Assert
            result.Should().Be(expectedResult);
            order.Products.Count.Should().Be(2);
            order.Products.Last().Id.Should().Be(SecondId);
            productFilter(_product).Should().BeFalse();
            productFilter(_secondProduct).Should().BeTrue();
            orderFilter(order).Should().BeTrue();
            orderFilter(invalidOrder).Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void OnAddingProducts_WithInvalidProducts_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsFalse()
        {
            //Arrange
            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new List<Product>());

            //Act
            var result = _service.AddProductsTo(FirstId, new List<int> { FirstId });

            //Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnAddingProducts_WithInvalidOrder_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsFalse()
        {
            //Arrange
            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new[] { _secondProduct });
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new List<Order>());

            //Act
            var result = _service.AddProductsTo(FirstId, new List<int> { SecondId });

            //Assert
            result.Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnRemovingProducts_WithValidProducts_ShouldRemove_AndUpdateTheRepository_AndReturnsTheQuantityDeleted()
        {
            //Arrange
            _products.Clear();
            _products.Add(_product);
            var productsToRemove = new List<Product> {_secondProduct, _thirdProduct};
            var order = new Order(_products.Concat(productsToRemove).ToList(), _customer) { Id = FirstId };
            var invalidOrder = new Order(_products, _customer) { Id = SecondId };
            Func<Product, bool> productFilter = null;
            Func<Order, bool> orderFilter = null;

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(productsToRemove).Callback<Func<Product, bool>>(filter => productFilter = filter);
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new[] { order }).Callback<Func<Order, bool>>(filter => orderFilter = filter);
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == FirstId &&
                    o.Products.Count == _products.Count &&
                    o.Products.First().Id == _product.Id && o.Products.First().Name == _product.Name && Math.Abs(o.Products.First().Value - _product.Value) == 0 &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion == null
                ))).Returns(FirstId);

            //Act
            var result = _service.RemoveProductsFrom(FirstId, new List<int> { SecondId, ThirdId });

            //Assert
            result.Should().Be(productsToRemove.Count);
            order.Products.Count.Should().Be(1);
            order.Products.Last().Id.Should().Be(FirstId);
            order.Products.Any(product => productsToRemove.Any(prod => prod.Id == product.Id)).Should().BeFalse();
            productFilter(_product).Should().BeFalse();
            productFilter(_secondProduct).Should().BeTrue();
            productFilter(_thirdProduct).Should().BeTrue();
            orderFilter(order).Should().BeTrue();
            orderFilter(invalidOrder).Should().BeFalse();
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void OnRemovingProducts_WithInvalidProducts_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsZero()
        {
            //Arrange
            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(new List<Product>());

            //Act
            var result = _service.RemoveProductsFrom(FirstId, new List<int> { SecondId, ThirdId });

            //Assert
            result.Should().Be(0);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnRemovingProducts_WithInvalidOrder_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsZero()
        {
            //Arrange
            var productsToRemove = new List<Product> { _secondProduct, _thirdProduct };

            _mockProductRepository.Setup(repo => repo.Get(It.IsAny<Func<Product, bool>>())).Returns(productsToRemove);
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new List<Order>());

            //Act
            var result = _service.RemoveProductsFrom(FirstId, new List<int> { SecondId, ThirdId });

            //Assert
            result.Should().Be(0);
            _mockProductRepository.Verify(repo => repo.Get(It.IsAny<Func<Product, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnApplyPromotion_WithInvalidPromotion_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsZero()
        {
            //Arrange
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new List<Promotion>());

            //Act
            var result = _service.ApplyPromotionTo(FirstId, FirstId);

            //Assert
            result.Should().Be(0);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [Test]
        public void OnApplyPromotion_WithInvalidOrder_ShouldNeverCallAddOrUpdateFromRepository_AndReturnsZero()
        {
            //Arrange
            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new[] { _orderPromotion });
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new List<Order>());

            //Act
            var result = _service.ApplyPromotionTo(FirstId, FirstId);

            //Assert
            result.Should().Be(0);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Never);
        }

        [TestCase(PromotionType.Order)]
        [TestCase(PromotionType.Product)]
        public void OnApplyPromotion_WithValidArguments_ShouldApplyThePromotion_UpdateTheRepository_AndReturnsTheValueDeducted(PromotionType type)
        {
            //Arrange
            _products.Clear();
            _products.Add(_product);
            _products.Add(_secondProduct);
            _products.Add(_thirdProduct);

            Promotion promotionToApply;
            Promotion invalidPromotion;
            double expectedDiscount;
            var promotionId = 0;
            switch (type)
            {
                case PromotionType.Product:
                    promotionToApply = _productPromotion;
                    invalidPromotion = _orderPromotion;
                    expectedDiscount = _secondProduct.Value * (_productPromotion.DiscountPercentage / 100D);
                    promotionId = promotionToApply.Id ?? SecondId;
                    break;
                case PromotionType.Order:
                    promotionToApply = _orderPromotion;
                    invalidPromotion = _productPromotion;
                    expectedDiscount = _products.Sum(prod => prod.Value) * (_orderPromotion.DiscountPercentage / 100D);
                    promotionId = promotionToApply.Id ?? FirstId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var totalAfterApplyPromotion = _products.Sum(prod => prod.Value) - expectedDiscount;
            var order = new Order(_products, _customer, promotionToApply) { Id = FirstId };
            var invalidOrder = new Order(_products, _customer) { Id = SecondId };
            Func<Promotion, bool> promotionFilter = null;
            Func<Order, bool> orderFilter = null;

            _mockPromotionRepository.Setup(repo => repo.Get(It.IsAny<Func<Promotion, bool>>())).Returns(new[] { promotionToApply }).Callback<Func<Promotion, bool>>(filter => promotionFilter = filter);
            _mockOrderRepository.Setup(repo => repo.Get(It.IsAny<Func<Order, bool>>())).Returns(new[] { order }).Callback<Func<Order, bool>>(filter => orderFilter = filter);
            _mockOrderRepository
                .Setup(repo => repo.AddOrUpdate(It.Is<Order>(o =>
                    o.Id == FirstId &&
                    o.Products.Count == _products.Count &&
                    o.Customer.Id == _customer.Id && o.Customer.Name == _customer.Name &&
                    o.Promotion.Id == promotionToApply.Id && o.Promotion.TargetId == promotionToApply.TargetId && o.Promotion.Type == promotionToApply.Type && o.Promotion.DiscountPercentage == promotionToApply.DiscountPercentage && o.Promotion.Description == promotionToApply.Description
                ))).Returns(FirstId);

            //Act
            var result = _service.ApplyPromotionTo(FirstId, promotionId);

            //Assert
            result.Should().Be(expectedDiscount);
            orderFilter(order).Should().BeTrue();
            orderFilter(invalidOrder).Should().BeFalse();
            promotionFilter(promotionToApply).Should().BeTrue();
            promotionFilter(invalidPromotion).Should().BeFalse();
            order.Total.Should().Be(totalAfterApplyPromotion);
            _mockPromotionRepository.Verify(repo => repo.Get(It.IsAny<Func<Promotion, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.Get(It.IsAny<Func<Order, bool>>()), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddOrUpdate(It.IsAny<Order>()), Times.Once);

            //Reset
            _products.Clear();
            _products.Add(_product);
        }
    }
}
