#Creating a booking from ladder
##Step 1 

Retrieve all facilities from (GET) "booking-facilities/api/facilites". This will contain all facilities with the location and sports available. 
NOTE: Do not allow the user to pick the facility. This will be choosen when you specify a sport, venue and time.

##Step 2
Allow user to pick a venue and sport. Not all sports are available at all venues. It is venue specific. 
An API call to "/sports/getSportsByVenue/{venueId}" may help as it will return a list of sports that are available at a venue.

##Step 3
Allow the user to pick a date. Once you know the sport, venue and date. Make a call to "/booking/{date}/{venueId}/{sportId}". This will return all free times for that sport at that venue. If two facilities are available at 9:00, the time 9:00 will only be returned once.

#Step 4
Create a booking using the API "/booking/{venueId}/{sportId}" specify the venue and sport in route. Specify the booking datetime and the booking userid. The booking will be returned with the allocated facility.
