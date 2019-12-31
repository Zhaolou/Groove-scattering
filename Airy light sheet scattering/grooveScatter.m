function result = grooveScatter(lambda, incidentTheta, width, depth, x, z, pmn, qm)
    
    k0 = 2*pi/lambda;
    kx = k0 * cos(incidentTheta);
    a = width;
    d = depth;
    M = fix(4*a/lambda) + 2;
    Q = qm;
    P = pmn;
    P = P(1:M,1:M);
    Q = Q(1:M);
    C = (eye(M,M) - P)\Q;
    NN = 2048;
    hInterface = zeros(NN+1, 1);
    dix = 2*a/NN;
    for ii = 1:(NN+1)
        xx(ii) = (ii - 1 - NN/2)*dix;
        hInterface(ii) = 0;
        for mm = 1:M
           hInterface(ii) = hInterface(ii) + C(mm)*sin(kcm(lambda, a, mm)*d)*sin(am(a, mm)*(xx(ii) + a));
        end
    end
    ax = x; az = z;
    H = zeros(size(ax));
    for ii = 1:max(size(ax))
        ii = ii;
        for jj = 1:NN+1
            xx = (jj - 1 - NN/2)*dix;
            H(ii) = H(ii) + exp(1j*k0*az(ii))/1j/lambda/az(ii) * hInterface(jj) * exp(1j*k0/2/az(ii)*((ax(ii)-xx)^2))*dix *(2*pi*az(ii)/k0)^0.5;
        end
    end    
    result = H;
