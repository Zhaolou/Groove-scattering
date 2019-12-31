function result = QM(lambda, width, depth, incidentTheta)
    k0 = 2*pi/lambda;
    kz = k0 * sin(incidentTheta);
    kx = k0 * cos(incidentTheta);
    a = width;
    d = depth;
    M = fix(4*a/lambda)+2;

    Q = zeros(M,1);
    for ii = 1:M
        an = am(a,ii);
        kcn = kcm(lambda, a, ii);
        Q(ii) = -2*1j*kz*an*(-(-1)^ii*exp(1j*kx*a)+exp(-1j*kx*a))*exp(1j*kcn*d)/kcn/a/(am(a,ii)^2 - kx*kx);
                
    end
    result = Q; 

