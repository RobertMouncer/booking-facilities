echo "$ENCRYPTED_DOCKER_PASSWORD" | docker login -u "$ENCRYPTED_DOCKER_USERNAME" --password-stdin
cd booking-facilities
docker build -t sem56402018/booking-facilities:$1 -t sem56402018/booking-facilities:$TRAVIS_COMMIT .
docker push sem56402018/booking-facilities:$TRAVIS_COMMIT
docker push sem56402018/booking-facilities:$1