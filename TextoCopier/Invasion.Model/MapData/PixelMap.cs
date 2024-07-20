namespace Lyt.Invasion.Model.MapData;

public sealed class PixelMap
{
    private enum PositionEnum
    {
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft
    }

    // minimum distance from the border of a region's coordinate
    private const int padding = 15;
    private const short noCountry = short.MaxValue;

    /// <summary> 
    /// Coordinates of each region, which is the starting point for the algorithm assigning pixels to a region.
    /// </summary>
    private readonly Coordinate[] coordinatesByRegion;

    /// <summary>  Barycenter coordinates of each region. </summary>
    private readonly Coordinate[] centersByRegion;

    public List<Coordinate>[] borderCoordinatesByCountry;

    private readonly int[] sizeByRegion;

    private readonly bool [,] neighboursByRegion; 

    private readonly GameOptions gameOptions;

    private readonly Map map ;

    /// <summary> Count of regions </summary>
    public readonly int RegionCount;

    /// <summary> Count of map-pixels in x direction, largest V coordinate is XCount-1 </summary>
    public readonly int XCount;

    /// <summary>Largest possible x coordinate </summary>
    public readonly int XMax;

    /// <summary> Count of map-pixels in y direction, largest y coordinate is YCount-1</summary>
    public readonly int YCount;

    /// <summary> Largest possible y coordinate </summary>
    public readonly int YMax;

    /// <summary> The Id for each pixel </summary>
    /// <remarks> there are hundred thousands of pixels. It's better to store the Id as short</remarks>
    public short[,] RegionIdsPerPixel { get; private set; }

    public bool [,] IsBorderPixel { get; private set; }

#pragma warning disable IDE0044 // Add readonly modifier. isTestBorderProblem can be manually set during debugging
    private bool isTestBorderProblem;
#pragma warning restore IDE0044

    public PixelMap(GameOptions gameOptions, Map map, IMessenger messenger, ILogger logger)
    {
        this.gameOptions = gameOptions;
        this.map = map;
        this.Messenger = messenger;
        this.Logger = logger;
        this.Random = new Random(Environment.TickCount);
        this.RegionCount = this.gameOptions.RegionCount;
        this.XCount = this.gameOptions.PixelWidth;
        this.XMax = this.XCount - 1;
        this.YCount = this.gameOptions.PixelHeight;
        this.YMax = this.YCount - 1;
        this.RegionIdsPerPixel = new short[this.XCount, this.YCount];
        this.IsBorderPixel = new bool[this.XCount, this.YCount];

        this.coordinatesByRegion = new Coordinate[this.RegionCount];
        this.centersByRegion = new Coordinate[this.RegionCount];
        this.sizeByRegion = new int[this.RegionCount];
        this.neighboursByRegion = new bool[this.RegionCount, this.RegionCount];
        this.borderCoordinatesByCountry = new List<Coordinate>[this.RegionCount];
        this.isTestBorderProblem = false;

        this.ClearMap();
        this.GenerateRegionStartingPoints();
        this.GenerateRegionCoordinates();

        int retries = 5; 
        while (retries > 0 && this.VerifyNoSinglePixelAreas())
        {
            this.RemoveSinglePixelAreas();
            -- retries;
        }

        this.GenerateRegionBordersSizeCenters();
        this.CreateRegions();
        this.GenerateRegionNeighbours(); 
    }

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    // Random number generator used during creation of PixelMap
    public Random Random { get; private set; }

    /// <summary> Count of pixels the largest country occupies </summary>
    public double BiggestCountrySize { get; private set; }

    /// <summary> Indexer, returns the Id of the region owning that pixel </summary>
    public short this[Coordinate coordinate] => this.RegionIdsPerPixel[coordinate.X, coordinate.Y];

    public short RegionAt(int x, int y) => this.RegionIdsPerPixel[x, y];

    private void ClearMap()
    {
        for (int y = 0; y < this.YCount; ++y)
        {
            for (int x = 0; x < this.XCount; ++x)
            {
                this.RegionIdsPerPixel[x, y] = noCountry;
            }
        }
    }

    private void GenerateRegionStartingPoints()
    {
        // Place for each region a random starting point on the map
        for (int regionIndex = 0; regionIndex < this.RegionCount; ++regionIndex)
        {
            bool isTooClose;
            int loopCounter = 0;
            Coordinate coordinate1;
            do
            {
                coordinate1 =
                    new Coordinate(
                        this.Random.Next(padding, this.XMax - padding),
                        this.Random.Next(padding, this.YMax - padding));
                isTooClose = false;
                for (int regionIndex2 = 0; regionIndex2 < regionIndex; ++regionIndex2)
                {
                    var coordinate2 = this.coordinatesByRegion[regionIndex2];
                    if (coordinate1.GetSquareDistance(this, coordinate2) < 200)
                    {
                        // Closer than 20 pixels
                        isTooClose = true;
                        break;
                    }
                }

                ++loopCounter;
                if (loopCounter > 666)
                {
                    throw new Exception("failed 666 time to place a region.");
                }

            } while (isTooClose);

            this.coordinatesByRegion[regionIndex] = coordinate1;
        }
    }

    private void GenerateRegionCoordinates()
    {
        // Use the country coordinates as start pixel for map filling
        var fillCoordinatesByCountry = new List<Coordinate>[this.RegionCount];
        var borderPixelsByCountry = new HashSet<Coordinate>[this.RegionCount];

        // Create a list of fillCoordinates for every region, with the region's coordinate as start for every region
        for (int regionIndex = 0; regionIndex < this.RegionCount; ++regionIndex)
        {
            var countryCoordinates = new List<Coordinate>(4096) { this.coordinatesByRegion[regionIndex] };
            fillCoordinatesByCountry[regionIndex] = countryCoordinates;
            borderPixelsByCountry[regionIndex] = [];
        }

        // Fill the map
        bool isIncomplete;
        var workCoordinates = new List<Coordinate>(256);
        do
        {
            isIncomplete = false;
            for (int regionIndex = 0; regionIndex < this.RegionCount; ++regionIndex)
            {
                void OccupyIfAvailable(
                  Coordinate originCoordinate,
                  Func<Coordinate, Coordinate> GetNeighbouringPixel)
                {
                    int pixelIndexMax = this.Random.Next(1, 4);
                    for (int pixelIndex = 0; pixelIndex < pixelIndexMax; ++pixelIndex)
                    {
                        Coordinate nextCoordinate = GetNeighbouringPixel(originCoordinate);
                        short mapCountryId = this[nextCoordinate];
                        if (mapCountryId == noCountry)
                        {
                            // Empty pixel found
                            this.RegionIdsPerPixel[nextCoordinate.X, nextCoordinate.Y] = (short)regionIndex;
                            workCoordinates.Add(nextCoordinate);
                        }
                        else
                        {
                            if (mapCountryId != regionIndex + 1)
                            {
                                // neighbour found
                                borderPixelsByCountry[regionIndex].Add(originCoordinate);
                            }
                            break;
                        }

                        originCoordinate = nextCoordinate;
                    }
                }

                var countryCoordinates = fillCoordinatesByCountry[regionIndex];
                foreach (var coordinate in countryCoordinates)
                {
                    OccupyIfAvailable(coordinate, c => c.Left(this));
                    OccupyIfAvailable(coordinate, c => c.Up(this));
                    OccupyIfAvailable(coordinate, c => c.Right(this));
                    OccupyIfAvailable(coordinate, c => c.Down(this));
                }

                if (workCoordinates.Count > 0)
                {
                    isIncomplete = true;
                    fillCoordinatesByCountry[regionIndex].AddRange(workCoordinates);

                    // reuse this list for next country instead creating each time a new list
                    workCoordinates.Clear();
                }
            }
        } while (isIncomplete);
    }

    private void GenerateRegionBordersSizeCenters()
    {
        int loopCounter = 0;
        do
        {
            loopCounter++;
            this.Logger.Debug ("Processing map, loop #" + loopCounter);
            if (loopCounter > 7)
            {
                throw new Exception("Failed too many times to process this PixelMap.");
            }

            this.RemoveSinglePixelAreas();

            // Find border pixels, size and center
            bool[] isLeftBorderCountry = new bool[this.RegionCount];
            for (int x = 0; x < this.XCount; x++)
            {
                isLeftBorderCountry[this.RegionIdsPerPixel[x, 0]] = true;
            }
            bool[] isTopBorderCountry = new bool[this.RegionCount];
            for (int y = 0; y < this.YCount; y++)
            {
                isTopBorderCountry[this.RegionIdsPerPixel[0, y]] = true;
            }

            long[,] sumCoordinatesByCountry = new long[this.RegionCount, 2];
            int yHalf = this.YCount / 2;
            int xHalf = this.XCount / 2;
            for (int y = 0; y < this.YCount; ++y)
            {
                for (int x = 0; x < this.XCount; ++x)
                {
                    int countryId = this.RegionIdsPerPixel[x, y];
                    this.sizeByRegion[countryId]++;
                    var coordinate = new Coordinate(x, y);

                    void CheckNeighbour(Coordinate neighbourCoordinate)
                    {
                        int neighbourId = this[neighbourCoordinate];
                        if (countryId != neighbourId)
                        {
                            // border found
                            this.neighboursByRegion[neighbourId, countryId] = true;
                            this.neighboursByRegion[countryId, neighbourId] = true;
                        }
                    }

                    CheckNeighbour(coordinate.Right(this));
                    CheckNeighbour(coordinate.Down(this));

                    if (isTopBorderCountry[countryId] && x < xHalf)
                    {
                        sumCoordinatesByCountry[countryId, 0] += x + this.XCount;
                    }
                    else
                    {
                        sumCoordinatesByCountry[countryId, 0] += x;
                    }

                    if (isLeftBorderCountry[countryId] && y < yHalf)
                    {
                        sumCoordinatesByCountry[countryId, 1] += y + this.YCount;
                    }
                    else
                    {
                        sumCoordinatesByCountry[countryId, 1] += y;
                    }
                }
            }

            int xPadding = 0;
            if (this.XCount > 100)
            {
                xPadding = 15;
            }

            int yPadding = 0;
            if (this.YCount > 100)
            {
                yPadding = 15;
            }

            for (int regionIndex = 0; regionIndex < this.RegionCount; ++regionIndex)
            {
                int x = (int)(sumCoordinatesByCountry[regionIndex, 0] / this.sizeByRegion[regionIndex]);
                if (x > this.XMax)
                {
                    x -= this.XCount;
                }

                x = Math.Max(x, xPadding);
                x = Math.Min(x, this.XMax - xPadding);

                int y = (int)(sumCoordinatesByCountry[regionIndex, 1] / this.sizeByRegion[regionIndex]);
                if (y > this.YMax)
                {
                    y -= this.YCount;
                }

                y = Math.Max(y, yPadding);
                y = Math.Min(y, this.YMax - yPadding);
                this.centersByRegion[regionIndex] = new Coordinate(x, y);
            }

            this.borderCoordinatesByCountry = new List<Coordinate>[this.RegionCount];
        } while (!this.FindBorderLines(this.borderCoordinatesByCountry));

        #region Mountains : Later 
        //// sort countries by size
        //var countrySizeByIds = new List<Tuple<int, int>>(RegionCount);
        //for (var countryIndex = 0; countryIndex < RegionCount; countryIndex++)
        //{
        //    countrySizeByIds.Add(new Tuple<int, int>(countryIndex, sizeByCountry[countryIndex]));
        //}
        //var sortedCountrySizeByIds = countrySizeByIds.OrderByDescending(c => c.Item2).ToArray();
        //BiggestCountrySize = sortedCountrySizeByIds[0].Item2;
        //armiesPerSize = armiesInBiggestCountry / BiggestCountrySize;

        ////mark the smallest countries as mountains
        //var isMountainByCountry = new bool[RegionCount];
        //for (var sortedCountryIndex = sortedCountrySizeByIds.Length - mountainCount; sortedCountryIndex < sortedCountrySizeByIds.Length; sortedCountryIndex++)
        //{
        //    var countryIndex = sortedCountrySizeByIds[sortedCountryIndex].Item1;
        //    isMountainByCountry[countryIndex] = true;
        //}

        //// mark a country as mountain if it is surrounded by mountains only
        //for (var sortedCountryIndex = 0; sortedCountryIndex < sortedCountrySizeByIds.Length - mountainCount; sortedCountryIndex++)
        //{
        //    var countryIndex = sortedCountrySizeByIds[sortedCountryIndex].Item1;
        //    var allNeighboursAreMountains = true;
        //    for (var neighbourIndex = 0; neighbourIndex < RegionCount; neighbourIndex++)
        //    {
        //        if (neighbourIndex != countryIndex && neighboursByCountry[countryIndex, neighbourIndex])
        //        {
        //            if (!isMountainByCountry[neighbourIndex])
        //            {
        //                allNeighboursAreMountains = false;
        //                break;
        //            }
        //        }
        //    }
        //    if (allNeighboursAreMountains)
        //    {
        //        isMountainByCountry[countryIndex] = true;
        //    }
        //}

        #endregion Mountains : Later  

        //// create CountryFix
        //countryFixArray = new CountryFix[RegionCount];
        //for (var countryIndex = 0; countryIndex < RegionCount; countryIndex++)
        //{
        //    var countryFix = new CountryFix(countryIndex, coordinatesByCountry[countryIndex], centerByCountry[countryIndex],
        //      isMountainByCountry[countryIndex], sizeByCountry[countryIndex], (int)(sizeByCountry[countryIndex] * armiesPerSize),
        //      borderCoordinatesByCountry[countryIndex]);
        //    countryFixArray[countryIndex] = countryFix;
        //}

        // find neighbours
        // TODO 

        //for (var countryIndex = 0; countryIndex < this.RegionCount; countryIndex++)
        //{
        //    var countryFix = countryFixArray[countryIndex];
        //    if (countryFix.IsMountain)
        //        continue;

        //    for (var neighbourIndex = 0; neighbourIndex < countryIndex; neighbourIndex++)
        //    {
        //        if (neighboursByCountry[countryIndex, neighbourIndex])
        //        {
        //            var neighbour = countryFixArray[neighbourIndex];
        //            if (neighbour.IsMountain)
        //                continue;
        //            countryFix.AddNeighbour(neighbour);
        //            neighbour.AddNeighbour(countryFix);
        //        }
        //    }
        //}
    }

    private void RemovePixel(Coordinate pixel)
    {
        short countryId = this[pixel];
        var regionIdIdCounts = new Dictionary<short, int>(8);

        void Count(Coordinate pixel)
        {
            short pixelRegionId = this[pixel];
            regionIdIdCounts[pixelRegionId] = regionIdIdCounts.TryGetValue(pixelRegionId, out int count) ? count + 1 : 1;
        }

        var startPixel = pixel;
        pixel = pixel.Up(this);
        Count(pixel);
        pixel = pixel.Right(this);
        Count(pixel);
        pixel = pixel.Down(this);
        Count(pixel);
        pixel = pixel.Down(this);
        Count(pixel);
        pixel = pixel.Left(this);
        Count(pixel);
        pixel = pixel.Left(this);
        Count(pixel);
        pixel = pixel.Up(this);
        Count(pixel);
        pixel = pixel.Up(this);
        Count(pixel);

        int biggestCount = 0;
        int majorityRegionId = 0;
        foreach (var keyValuePair in regionIdIdCounts)
        {
            if (countryId != keyValuePair.Key && biggestCount < keyValuePair.Value)
            {
                biggestCount = keyValuePair.Value;
                majorityRegionId = keyValuePair.Key;
            }
        }

        this.RegionIdsPerPixel[startPixel.X, startPixel.Y] = (short)majorityRegionId;
    }

    private void RemoveSinglePixelAreas()
    {
        // remove all single pixel wide areas.
        // They cause problems when tracing borders and look ugly
        for (int y = 0; y < this.YCount; y++)
        {
            for (int x = 0; x < this.XCount; x++)
            {
                int regionId = this.RegionIdsPerPixel[x, y];
                var pixel = new Coordinate(x, y);
                if (
                  (this[pixel.Up(this)] != regionId && this[pixel.Down(this)] != regionId) ||
                  (this[pixel.Left(this)] != regionId && this[pixel.Right(this)] != regionId))
                {
                    this.RemovePixel(pixel);

                    Coordinate prev1Pixel;
                    Coordinate prev2Pixel;
                    if (pixel.X > 0)
                    {
                        //check pixel left. There is no need to check pixel 0, because pixel XMax will be tested later
                        prev1Pixel = pixel.Left(this);
                        prev2Pixel = prev1Pixel.Left(this);
                        if (this[prev1Pixel] == regionId && this[prev2Pixel] != regionId)
                        {
                            // there were 2 'single' pixels in a row with the same regionId.
                            // Since the second got removed, the first is now single and needs to be removed too:
                            //....
                            //.21.
                            //.*..
                            //****
                            // after 1 gets removed, 2 becomes a single pixel area and must be removed too.
                            // Note: '1', '2' and '*' have the same regionId, but different from '.'
                            this.RemovePixel(prev1Pixel);
                        }
                    }

                    if (pixel.Y > 0)
                    {
                        //check pixel above
                        prev1Pixel = pixel.Up(this);
                        prev2Pixel = prev1Pixel.Up(this);
                        if (this[prev1Pixel] == regionId && this[prev2Pixel] != regionId)
                        {
                            this.RemovePixel(prev1Pixel);
                        }
                    }
                }

                // If connection between 2 country regions is only 1 pixel, add another pixel.
                // Otherwise border tracing gets stuck in an endless loop
                //***..       ***..
                //**1..       **1**
                //..***   or  ...**
                //..***       ...**
            }
        }
    }

    private bool FindBorderLines(List<Coordinate>[] borderCoordinatesByCountry)
    {
        bool hasFound = true;
        for (int countryIndex = 0; countryIndex < this.coordinatesByRegion.Length; countryIndex++)
        {
            Coordinate startPixel = this.coordinatesByRegion[countryIndex];
            Coordinate lastStartPixel;
            bool isSearchUp = true;
            int stepCount = 0;

            // search up or right, until a different country is found
            do
            {
                lastStartPixel = startPixel;
                startPixel = isSearchUp ? startPixel.Up(this) : startPixel.Right(this);
                stepCount++;
                if (stepCount == this.YCount)
                {
                    // no border found searching vertically. search horizontally
                    isSearchUp = false;
                }

                if (stepCount > this.YCount + this.XCount)
                {
                    throw new Exception("No border found for country ID: " + countryIndex);
                }
            } while (this[startPixel] == countryIndex);

            // first border pixel found
            startPixel = lastStartPixel;
            Coordinate pixel = startPixel;
            var borderCoordinates = new List<Coordinate>(1024);
            var lastCoordinates = new Coordinate[20];
            int lastCoordinatesIndex;
            for (lastCoordinatesIndex = 0; lastCoordinatesIndex < lastCoordinates.Length; lastCoordinatesIndex++)
            {
                lastCoordinates[lastCoordinatesIndex] = new Coordinate(-1,-1);
            }

            lastCoordinatesIndex = 0;

            #region Search Next Pixel 

            void SearchNextPixelInside(ref Coordinate pixel)
            {
                //search counter clock wise from Top for pixel different from countryId
                //use second last pixel, which still has the countryId
                var position = PositionEnum.Top;
                Coordinate lastPixel;
                do
                {
                    //turn counter clockwise
                    lastPixel = pixel;
                    switch (position)
                    {
                        case PositionEnum.Top:
                            position = PositionEnum.TopLeft;
                            pixel = pixel.Left(this);
                            break;

                        case PositionEnum.TopRight:
                            throw new Exception("none of the neighbouring pixels have the country Id: " + countryIndex + ".");

                        case PositionEnum.Right:
                            position = PositionEnum.TopRight;
                            pixel = pixel.Up(this);
                            break;

                        case PositionEnum.BottomRight:
                            position = PositionEnum.Right;
                            pixel = pixel.Up(this);
                            break;

                        case PositionEnum.Bottom:
                            position = PositionEnum.BottomRight;
                            pixel = pixel.Right(this);
                            break;

                        case PositionEnum.BottomLeft:
                            position = PositionEnum.Bottom;
                            pixel = pixel.Right(this);
                            break;

                        case PositionEnum.Left:
                            position = PositionEnum.BottomLeft;
                            pixel = pixel.Down(this);
                            break;

                        case PositionEnum.TopLeft:
                            position = PositionEnum.Left;
                            pixel = pixel.Down(this);
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                } while (this.RegionIdsPerPixel[pixel.X, pixel.Y] == countryIndex);

                pixel = lastPixel; // lastPixel has still countryIndex
            }

            void SearchNextPixelOutside(ref Coordinate pixel)
            {
                // search clock wise from Top for countryId
                // use last pixel found, it has already the countryId
                var position = PositionEnum.Top;
                do
                {
                    //turn clockwise
                    switch (position)
                    {
                        case PositionEnum.Top:
                            position = PositionEnum.TopRight;
                            pixel = pixel.Right(this);
                            break;
                        case PositionEnum.TopRight:
                            position = PositionEnum.Right;
                            pixel = pixel.Down(this);
                            break;
                        case PositionEnum.Right:
                            position = PositionEnum.BottomRight;
                            pixel = pixel.Down(this);
                            break;
                        case PositionEnum.BottomRight:
                            position = PositionEnum.Bottom;
                            pixel = pixel.Left(this);
                            break;
                        case PositionEnum.Bottom:
                            position = PositionEnum.BottomLeft;
                            pixel = pixel.Left(this);
                            break;
                        case PositionEnum.BottomLeft:
                            position = PositionEnum.Left;
                            pixel = pixel.Up(this);
                            break;
                        case PositionEnum.Left:
                            position = PositionEnum.TopLeft;
                            pixel = pixel.Up(this);
                            break;
                        case PositionEnum.TopLeft:
                            throw new Exception("all neighbouring pixels have the country countryId: " + countryIndex + ".");
                        default:
                            throw new NotSupportedException();
                    }
                } while (this.RegionIdsPerPixel[pixel.X, pixel.Y] != countryIndex);
            }

            bool ArePixelRepeating(Coordinate pixel, Coordinate[] lastCoordinates, ref int lastCoordinatesIndex)
            {
                // check if Pixel is in lastCoordinates
                int sameIndex;
                for (sameIndex = 0; sameIndex < lastCoordinates.Length; sameIndex++)
                {
                    if (lastCoordinates[sameIndex].Equals(pixel))
                    {
                        break;
                    }
                }

                if (sameIndex >= lastCoordinates.Length)
                {
                    lastCoordinates[lastCoordinatesIndex] = pixel;
                    lastCoordinatesIndex = (lastCoordinatesIndex + 1) % lastCoordinates.Length;
                    return false;
                }

                // same pixel was processed before. Remove the remaining pixels from the map
                int  removeIndex = (sameIndex + 1) % lastCoordinates.Length; //leave the first repeated pixel
                if (removeIndex == lastCoordinatesIndex)
                {
                    throw new Exception("The loop consists of only 1 pixel. This can not happen, but checking anyway.");
                }

                do
                {
                    this.RemovePixel(lastCoordinates[removeIndex]);
                    lastCoordinates[removeIndex] = new Coordinate(-1,-1);
                    removeIndex = (removeIndex + 1) % lastCoordinates.Length;
                } while (removeIndex != lastCoordinatesIndex);

                return true;
            }

            #endregion Search Next Pixel 

            do
            {
                //move to pixel above current pixel
                pixel = pixel.Up(this);
                if (this.RegionIdsPerPixel[pixel.X, pixel.Y] == countryIndex)
                {
                    // top pixel has  countryId, search counter clock wise for different countryId
                    SearchNextPixelInside(ref pixel);
                }
                else
                {
                    // top pixel has different countryId, search clock wise for countryId
                    SearchNextPixelOutside(ref pixel);
                }

                if (ArePixelRepeating(pixel, lastCoordinates, ref lastCoordinatesIndex))
                {
                    //tracing the border line is stuck in a loop. The problematic pixels were removed from this country
                    //and the processing of the map has to be repeated, since the map has changed
                    pixel = startPixel;
                    hasFound = false;
                }

                this.IsBorderPixel[pixel.X, pixel.Y] = true;
                borderCoordinates.Add(pixel);

                if (borderCoordinates.Count > 10000)
                {
//#if debugBorderCoordinates
//                        string s;
//                        if (coordinatesbyCountry.Length<92) {
//                          s = ToPlainString(countryIdsPerPixel, pixel, 5);
//                        }
//#endif
                    throw new Exception("too many border points. Press \"New Game\" to create a new game.");
                }
            } while (!pixel.Equals(startPixel));

            if (this.isTestBorderProblem)
            {
                //    int firstPixelIndex;
                //    var isLastPixelAtBottomBorder = false;
                //    for (firstPixelIndex = 0; firstPixelIndex < borderCoordinates.Count; firstPixelIndex++)
                //    {
                //        var y = borderCoordinates[firstPixelIndex].Y;
                //        if (isLastPixelAtBottomBorder)
                //        {
                //            if (y == 0)
                //            {
                //                //border of country as switched from bottom border of window to top border of window
                //                //for testing for border problem, shift border pixels so that it starts with y=0
                //                var borderCoordinatesCopy = new List<Coordinate>(borderCoordinates.Count);
                //                for (var copyPixelIndex = 0; copyPixelIndex < borderCoordinates.Count; copyPixelIndex++)
                //                {
                //                    borderCoordinatesCopy.Add(borderCoordinates[(firstPixelIndex + copyPixelIndex) % borderCoordinates.Count]);
                //                }
                //                borderCoordinates = borderCoordinatesCopy;
                //                break;
                //            }
                //        }
                //        isLastPixelAtBottomBorder = y == YMax;
                //    }
            }

            borderCoordinatesByCountry[countryIndex] = borderCoordinates;
        }

        return hasFound;
    }

    private void CreateRegions()
    {
        for (short regionIndex = 0; regionIndex < this.RegionCount; regionIndex++)
        {
            var region = new Region(
                regionIndex, this.coordinatesByRegion[regionIndex], this.centersByRegion[regionIndex],
                this.sizeByRegion[regionIndex], this.borderCoordinatesByCountry[regionIndex]);
            this.map.AddRegionAt(regionIndex, region);
        }
    }

    private void GenerateRegionNeighbours()
    {
        // find neighbours
        var regions = this.map.Regions;
        for (short regionIndex = 0; regionIndex < this.RegionCount; regionIndex++)
        {
            var region = regions[regionIndex];
            for (short neighbourIndex = 0; neighbourIndex < regionIndex; neighbourIndex++)
            {
                if (this.neighboursByRegion[regionIndex, neighbourIndex])
                {
                    var neighbour = regions[neighbourIndex];
                    region.AddNeighbour(neighbour);
                    neighbour.AddNeighbour(region);
                }
            }
        }
    }

    /// <summary> Verify Single Pixel Areas </summary>
    /// <returns> True if any </returns>
    private bool VerifyNoSinglePixelAreas()
    {
        // Check if there are still "single" pixels, i.e. without neighbours with the same countryId
        int count = 0;
        for (int y = 0; y < this.YCount; ++y)
        {
            for (int x = 0; x < this.XCount; ++x)
            {
                int countryId = this.RegionIdsPerPixel[x, y];
                var pixel = new Coordinate(x, y);
                if ((this[pixel.Up(this)] != countryId && this[pixel.Down(this)] != countryId) ||
                  (this[pixel.Left(this)] != countryId && this[pixel.Right(this)] != countryId))
                {
                    ++count;
                }
            }
        }

        if (count > 0)
        {
            // if (Debugger.IsAttached) { Debugger.Break(); }
            this.Logger.Debug( "Single Pixels Count: " + count.ToString() );
            return true;
        }

        return false;
    }
}
