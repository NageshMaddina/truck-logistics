<#
.SYNOPSIS
  Seeds sample carriers, drivers and loads through the TruckLogix API.
.DESCRIPTION
  Requires the API to be running (dotnet run in backend/TruckLogistics.API).
  Loads are walked through the real workflow (book -> pick up -> deliver) so
  statuses, driver availability and tracking history stay consistent.
  Skips seeding if the database already has carriers, unless -Force is given.
.EXAMPLE
  ./scripts/seed-sample-data.ps1
  ./scripts/seed-sample-data.ps1 -ApiUrl http://localhost:5050/api -Force
#>
param(
  [string]$ApiUrl = 'http://localhost:5050/api',
  [switch]$Force
)

$ErrorActionPreference = 'Stop'
$api = $ApiUrl.TrimEnd('/')

$existing = @(Invoke-RestMethod "$api/carriers" | ForEach-Object { $_ })
if ($existing.Count -gt 0 -and -not $Force) {
  Write-Host "Database already has $($existing.Count) carrier(s); skipping. Use -Force to seed anyway."
  return
}
function Post($path, $body) { Invoke-RestMethod -Method Post -Uri "$api/$path" -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 5) }
function Put($path, $body)  { Invoke-RestMethod -Method Put  -Uri "$api/$path" -ContentType 'application/json' -Body ($body | ConvertTo-Json -Depth 5) }
function Patch($path, $raw) { Invoke-RestMethod -Method Patch -Uri "$api/$path" -ContentType 'application/json' -Body $raw }

# --- Carriers ---
$carrierData = @(
  @{ name='Swift Haul Transport'; mcNumber='MC-784512'; dotNumber='2987451'; contactName='Linda Park';    contactEmail='dispatch@swifthaul.example';  contactPhone='(614) 555-0142'; address='1200 Freight Way';  city='Columbus';     state='OH'; zipCode='43215'; rating=4.8 },
  @{ name='Lone Star Freight';    mcNumber='MC-652390'; dotNumber='3120987'; contactName='Carlos Mendez'; contactEmail='ops@lonestarfreight.example'; contactPhone='(214) 555-0187'; address='88 Interstate Dr';  city='Dallas';       state='TX'; zipCode='75201'; rating=4.5 },
  @{ name='Great Lakes Carriers'; mcNumber='MC-903417'; dotNumber='2764390'; contactName='Emily Novak';   contactEmail='loads@greatlakes.example';    contactPhone='(312) 555-0119'; address='450 Harbor Rd';     city='Chicago';      state='IL'; zipCode='60607'; rating=4.2 },
  @{ name='Peach State Logistics';mcNumber='MC-418866'; dotNumber='3345120'; contactName='Marcus Hill';   contactEmail='dispatch@peachstate.example'; contactPhone='(404) 555-0163'; address='7 Terminal Blvd';   city='Atlanta';      state='GA'; zipCode='30303'; rating=4.6 }
)
$carriers = $carrierData | ForEach-Object { Post 'carriers' $_ }

# --- Drivers (2 per carrier) ---
$driverData = @(
  @('James','Carter'), @('Maria','Lopez'), @('Robert','Nguyen'), @('Aisha','Johnson'),
  @('Tom','Becker'),   @('Priya','Shah'),  @('Derek','Wilson'),  @('Hannah','Kim')
)
$drivers = @()
for ($i = 0; $i -lt $driverData.Count; $i++) {
  $c = $carriers[[math]::Floor($i / 2)]
  $f, $l = $driverData[$i]
  $drivers += Post 'drivers' @{ carrierId=$c.id; firstName=$f; lastName=$l; licenseNumber=('CDL-{0}{1:D6}' -f $c.state, (100231 + $i * 7919)); phone=('(555) 555-01{0:D2}' -f (20 + $i)); email=("{0}.{1}@example.com" -f $f.ToLower(), $l.ToLower()) }
}

# --- Loads ---
# target: Available | Booked | InTransit | Delivered ; d = driver index
$today = (Get-Date).Date
$loadData = @(
  @{ eq='Dry Van';  wt=42000; com='Consumer Electronics'; pa='500 Commerce St';  pc='Columbus';     ps='OH'; pz='43215'; da='2100 Logistics Pkwy'; dc='Chicago';     ds='IL'; dz='60607'; off=-9; days=1; mi=355;  rate=1250; target='Delivered'; d=0 },
  @{ eq='Reefer';   wt=38000; com='Frozen Foods';         pa='12 Cold Storage Ln'; pc='Dallas';     ps='TX'; pz='75201'; da='800 Market Ave';      dc='Atlanta';     ds='GA'; dz='30303'; off=-7; days=2; mi=780;  rate=2900; target='Delivered'; d=2 },
  @{ eq='Flatbed';  wt=45000; com='Steel Coils';          pa='3300 Mill Rd';       pc='Gary';        ps='IN'; pz='46402'; da='19 Industrial Blvd';  dc='Nashville';   ds='TN'; dz='37203'; off=-5; days=2; mi=470;  rate=1850; target='Delivered'; d=4 },
  @{ eq='Dry Van';  wt=30000; com='Paper Products';       pa='77 Warehouse Dr';    pc='Atlanta';     ps='GA'; pz='30303'; da='450 Port Rd';         dc='Charlotte';   ds='NC'; dz='28202'; off=-1; days=1; mi=245;  rate=950;  target='InTransit'; d=6 },
  @{ eq='Reefer';   wt=40000; com='Fresh Produce';        pa='9 Farm Market Rd';   pc='Fresno';      ps='CA'; pz='93721'; da='600 Distribution Way';dc='Denver';      ds='CO'; dz='80202'; off=-1; days=3; mi=1150; rate=3800; target='InTransit'; d=3 },
  @{ eq='Dry Van';  wt=35000; com='Auto Parts';           pa='1400 Assembly Ave';  pc='Detroit';     ps='MI'; pz='48226'; da='250 Plant Rd';        dc='Columbus';    ds='OH'; dz='43215'; off=1;  days=1; mi=205;  rate=800;  target='Booked';    d=1 },
  @{ eq='Flatbed';  wt=47000; com='Lumber';               pa='18 Timber Ln';       pc='Houston';     ps='TX'; pz='77002'; da='95 Builder St';       dc='Phoenix';     ds='AZ'; dz='85004'; off=2;  days=3; mi=1175; rate=3500; target='Booked';    d=5 },
  @{ eq='Dry Van';  wt=28000; com='Retail Apparel';       pa='320 Fashion Blvd';   pc='Los Angeles'; ps='CA'; pz='90012'; da='700 Outlet Dr';       dc='Las Vegas';   ds='NV'; dz='89101'; off=3;  days=1; mi=270;  rate=1100; target='Available'; d=$null },
  @{ eq='Reefer';   wt=36000; com='Dairy Products';       pa='44 Creamery Rd';     pc='Madison';     ps='WI'; pz='53703'; da='1500 Grocery Ctr';    dc='Minneapolis'; ds='MN'; dz='55401'; off=4;  days=1; mi=270;  rate=1150; target='Available'; d=$null },
  @{ eq='Step Deck';wt=44000; com='Construction Equipment';pa='60 Heavy Iron Way'; pc='Kansas City'; ps='MO'; pz='64105'; da='12 Site Access Rd';   dc='Omaha';       ds='NE'; dz='68102'; off=5;  days=1; mi=190;  rate=1400; target='Available'; d=$null }
)

foreach ($x in $loadData) {
  $pickup   = $today.AddDays($x.off).AddHours(8)
  $delivery = $pickup.AddDays($x.days).AddHours(6)
  $body = [ordered]@{
    equipmentType=$x.eq; weight=$x.wt; commodity=$x.com
    pickupAddress=$x.pa; pickupCity=$x.pc; pickupState=$x.ps; pickupZip=$x.pz; pickupDate=$pickup.ToString('s')
    deliveryAddress=$x.da; deliveryCity=$x.dc; deliveryState=$x.ds; deliveryZip=$x.dz; deliveryDate=$delivery.ToString('s')
    miles=$x.mi; rate=$x.rate; notes=$null
  }
  $load = Post 'loads' $body
  if ($x.target -eq 'Available') { continue }

  $drv = $drivers[$x.d]
  $upd = [ordered]@{} + $body
  $upd.status = 'Booked'; $upd.carrierId = $drv.carrierId; $upd.driverId = $drv.id
  Put "loads/$($load.id)" $upd
  if ($x.target -eq 'Booked') { continue }

  Post "loads/$($load.id)/tracking" @{ eventType='PickedUp'; location=$x.pa; city=$x.pc; state=$x.ps; notes='Loaded and sealed'; eventTime=$pickup.AddHours(1).ToString('s') } | Out-Null
  Post "loads/$($load.id)/tracking" @{ eventType='InTransit'; city=$x.pc; state=$x.ps; notes='En route'; eventTime=$pickup.AddHours(5).ToString('s') } | Out-Null
  if ($x.target -eq 'InTransit') { continue }

  Post "loads/$($load.id)/tracking" @{ eventType='Delivered'; location=$x.da; city=$x.dc; state=$x.ds; notes='Delivered, POD signed'; eventTime=$delivery.ToString('s') } | Out-Null
  Patch "drivers/$($drv.id)/availability" 'true' | Out-Null   # driver free again after delivery
}

"Seeded: $($carriers.Count) carriers, $($drivers.Count) drivers, $($loadData.Count) loads"
Invoke-RestMethod "$api/loads/stats" | ConvertTo-Json -Compress

