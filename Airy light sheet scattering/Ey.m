function result = Ex(x,z,x0,z0)
    result = 0;
    N = 100;
    lambda = 350e-9; k0 = 2*pi/lambda;
    for nx = -1:1/N:1
        nz = sqrt(1-nx^2); 
        result = result + nz*frequent(nx)*exp(1i*k0*(nx*(x-x0)+nz*(z-z0)))/N;
    end
end