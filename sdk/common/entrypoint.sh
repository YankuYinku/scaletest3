#!/usr/bin/env sh

# Make sure line endings are "LF" (Linux) and not "CRLF" (windows). Otherwise you get the error
# > env: can't execute 'sh
# > ': No such file or directory

set -eu

function join_by { local IFS="$1"; shift; echo "$*"; }

# Find vue env vars
vars=$(env | grep PORTAL_SDK_COMMON | awk -F = '{print "$"$1}')
vars=$(join_by ' ' $vars)
echo "Found variables $vars"

for file in /usr/share/nginx/html/*.js;
do
  echo "Processing $file ...";

  # Use the existing JS file as template
  cp $file $file.tmpl
  envsubst "$vars" < $file.tmpl > $file
  rm $file.tmpl
done

exec "$@"
