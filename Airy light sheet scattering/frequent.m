%%%%%%%%%%%%%%Airy light sheet
function result = frequent(lambda,nx,x0,z0)
    a = 1;
    xr = 0.35e-6; a0 = 0.1;
    k0 = 2*pi/lambda;
    
    
    nz = sqrt(1-nx.*nx);
    result = 1/2*exp(-a0*xr^2*k0^2*nx.^2).*exp(1i/3*(xr^3*k0^3*nx.^3-3*a0^2*xr*k0*nx-1i*a0^3))*exp(1i*k0*(nx*x0 + nz*z0));
end


%%%%%%%Gaussian light sheet
% function result = frequent(lambda,nx,x0,z0)						
%     w0 = 0.35e-6;
%     k0 = 2*pi/lambda;
%     l = 1/k0/w0;
%     nz = sqrt(1-nx.*nx);
%     result = 1/sqrt(pi)/l*exp(-nx.^2/l/l).*exp(1i*k0*(nx*x0 + nz*z0));
% %     if nx ~= 0
% %         result = 0;
% %     else
% %         result = 1/2;
% %     end
% end