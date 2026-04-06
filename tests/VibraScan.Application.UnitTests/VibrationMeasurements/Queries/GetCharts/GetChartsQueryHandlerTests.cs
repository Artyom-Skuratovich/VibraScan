using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;
using System.Runtime.InteropServices;
using VibraScan.Application.Common.Interfaces;
using VibraScan.Application.VibrationMeasurements.Queries.GetCharts;
using VibraScan.Domain.Entities;
using VibraScan.Domain.ValueObjects;

namespace VibraScan.Application.UnitTests.VibrationMeasurements.Queries.GetCharts
{
    public class GetChartsQueryHandlerTests
    {
        private IApplicationDbContext _context;
        private GetChartsQueryHandler _handler;
        private readonly DateTime _defaultCapturedAt = new(2026, 4, 6);

        [SetUp]
        public void SetUp()
        {
            _context = Substitute.For<IApplicationDbContext>();
            _handler = new GetChartsQueryHandler(_context);
        }

        [Test]
        public async Task HandleShouldReturnOnlyTimeChartWhenFrequencyDataDoesNotExist()
        {
            // Arrange
            var measurements = new List<VibrationMeasurement> { CreateMeasurement(MeasurementDomain.Time) };
            SetupContext(measurements);

            // Act
            var result = await _handler.Handle(CreateDefaultQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.TimeDomainChart.Should().NotBeNull();
            result.FrequencyDomainChart.Should().BeNull();
            result.TimeDomainChart!.Values.Should().NotBeEmpty();
            result.TimeDomainChart!.MeasurementDomain.Should().Be(MeasurementDomain.Time.Name);
        }

        [Test]
        public async Task HandleShouldReturnOnlyFrequencyChartWhenTimeDataDoesNotExist()
        {
            // Arrange
            var measurements = new List<VibrationMeasurement> { CreateMeasurement(MeasurementDomain.Frequency) };
            SetupContext(measurements);

            // Act
            var result = await _handler.Handle(CreateDefaultQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FrequencyDomainChart!.Should().NotBeNull();
            result.TimeDomainChart.Should().BeNull();
            result.FrequencyDomainChart!.Values.Should().NotBeEmpty();
            result.FrequencyDomainChart!.MeasurementDomain.Should().Be(MeasurementDomain.Frequency.Name);
        }

        [Test]
        public async Task HandleShouldReturnBothChartsWhenBothExist()
        {
            // Arrange
            var measurements = new List<VibrationMeasurement>
            {
                CreateMeasurement(MeasurementDomain.Time),
                CreateMeasurement(MeasurementDomain.Frequency)
            };
            SetupContext(measurements);

            // Act
            var result = await _handler.Handle(CreateDefaultQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TimeDomainChart.Should().NotBeNull();
            result.FrequencyDomainChart.Should().NotBeNull();
        }

        [Test]
        public async Task HandleShouldReturnNullWhenRawDataIsEmpty()
        {
            // Arrange
            var measurement = CreateMeasurement(MeasurementDomain.Time);
            measurement.RawData = [];

            SetupContext([measurement]);

            // Act
            var result = await _handler.Handle(CreateDefaultQuery(), CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task HandleShouldCalculateCorrectFrequencyAmplitudeRangeFromRawData()
        {
            // Arrange
            var values = new float[] { 2.5f, 10.0f, 0.5f, 7.0f, 4.0f };
            var rawData = MemoryMarshal.AsBytes(values.AsSpan()).ToArray();

            var measurement = CreateMeasurement(MeasurementDomain.Frequency);
            measurement.RawData = rawData;

            SetupContext([measurement]);

            // Act
            var result = await _handler.Handle(CreateDefaultQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FrequencyDomainChart.Should().NotBeNull();

            result.FrequencyDomainChart!.AmplitudeRange.Should().Be(9.5f);
        }

        private void SetupContext(ICollection<VibrationMeasurement> measurements)
        {
            var mockDbSet = measurements.BuildMockDbSet();
            _context.VibrationMeasurements.Returns(mockDbSet);
        }

        private VibrationMeasurement CreateMeasurement(MeasurementDomain domain)
        {
            var values = new float[] { 1.0f, 2.0f, 15.0f, 33.9f, 29.3f, 13.8f, 3.4f };
            var rawData = MemoryMarshal.AsBytes(values.AsSpan()).ToArray();

            return new VibrationMeasurement
            {
                PointId = 1,
                MeasurementProfileId = 1,
                AxisType = AxisType.Axial,
                CapturedAt = _defaultCapturedAt,
                MeasurementDomain = domain,
                RawData = rawData,
                Rms = 3.5f
            };
        }

        private GetChartsQuery CreateDefaultQuery() => new(_defaultCapturedAt, 1, 1, AxisType.Axial);
    }
}