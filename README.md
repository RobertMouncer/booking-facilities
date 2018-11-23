# booking-facilities
AberFitness Booking & Facilities Management Service

| Branch | Status |
|-|-|
| Development | [![Development Build Status](https://travis-ci.org/sem5640-2018/booking-facilities.svg?branch=development)](https://travis-ci.org/sem5640-2018/booking-facilities) |
| Release | [![Release Build Status](https://travis-ci.org/sem5640-2018/booking-facilities.svg?branch=master)](https://travis-ci.org/sem5640-2018/booking-facilities) |

# Maintained by
* Rob
* Charlie

# Objectives
* Admin UI for managing facilities
* Administrative booking of facilities (for special reasons, e.g. maintenance) 
* Users can book facilities
* API for bookings to hook into Ladder system

# Environment Variables

## Required Keys (All Environments)

| Environment Variable | Default | Description |
|-|-|-|
| ASPNETCORE_ENVIRONMENT | Production | Runtime environment, should be 'Development', 'Staging', or 'Production'. |
| ConnectionStrings__booking_facilitiesContext | N/A | MariaDB connection string. |

## Required Keys (Production + Staging Environments)
In addition to the above keys, you will also require:

| Environment Variable | Default | Description |
|-|-|-|
| Kestrel__Certificates__Default__Path | N/A | Path to the PFX certificate to use for HTTPS. |
| Kestrel__Certificates__Default__Password | N/A | Password for the HTTPS certificate. |
| Booking_Facilities__ReverseProxyHostname | http://nginx | The internal docker hostname of the reverse proxy being used. |
| Booking_Facilities__PathBase | /booking-facilities | The pathbase (name of the directory) that the app is being served from. |