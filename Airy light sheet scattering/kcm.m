function result = kcm(lambda,width, m)
a = width;
k0 = 2*pi/lambda;
result = sqrt(k0*k0-(m*pi/2/a)^2);