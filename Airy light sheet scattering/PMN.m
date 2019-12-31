function pmn = PMN(lambda, width, depth)
    k0 = 2*pi/lambda;
    a = width;
    d = depth;
    M = fix(4*a/lambda) + 2;

    P = zeros(M,M);
    index = 1;
    for ii = 1:M
        for jj = 1:M
            alpha = am(a,ii)/k0; beta = am(a,jj)/k0;
            sumI1 = 0; sumI2 = 0;
            dv = 1e-3;
            for v = dv:dv:20/(k0*a)
                    temp = k0^2* ((1+1j*v)^2 - alpha^2)*((1+1j*v)^2 - beta^2) ;
                    tI1 = -4*1j*(-1)^jj*exp(2*1j*a*k0)*exp(-2*v*a*k0)* sqrt(v*(-2*1j+v));
                    index= index + 1;
                    sumI1 = sumI1 + tI1/temp*dv;
            end
            dv = 2e-2;
            for v = 20/(k0*a) + dv:dv:100/(k0*a)
                    temp = k0^2* ((1+1j*v)^2 - alpha^2)*((1+1j*v)^2 - beta^2);
                    tI1 = -4*1j*(-1)^jj*exp(2*1j*a*k0)*exp(-2*v*a*k0)* sqrt(v*(-2*1j+v));
                    index= index + 1;
                    sumI1 = sumI1 + tI1/temp*dv;
            end
            dv = 1;
            for v = 100/(k0*a) + dv:dv:500/(k0*a)
                    temp = k0^2* ((1+1j*v)^2 - alpha^2)*((1+1j*v)^2 - beta^2);
                    tI1 = -4*1j*(-1)^jj*exp(2*1j*a*k0)*exp(-2*v*a*k0)* sqrt(v*(-2*1j+v));
                    index= index + 1;
                    sumI1 = sumI1 + tI1/temp*dv;
            end
  
            
            
            
            I1 = sumI1; 
            if alpha~= beta
                I2 = -4*1j/k0/k0/(alpha^2-beta^2)*(-sqrt(1-alpha^2)/alpha*asin(alpha)+sqrt(1-beta^2)/beta*asin(beta));
            else
                alpha = beta + 1e-10;
                I2 = -4*1j/k0/k0/(alpha^2-beta^2)*(-sqrt(1-alpha^2)/alpha*asin(alpha)+sqrt(1-beta^2)/beta*asin(beta));  
            end
            kcj = kcm(lambda, a, jj);
            kci = kcm(lambda, a, ii);
             P(ii,jj) = -1j*am(a,ii)*am(a,jj)*sin(kcj*d)/(kci*cos(kci*d) - 1j*kci*sin(kci*d))/2/pi/a*(I1+I2);
             if(abs(P(ii,jj)) > 1e5)
                 ii = ii;
             end
        end
    end
    pmn = P; 
